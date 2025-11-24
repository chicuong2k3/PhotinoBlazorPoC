using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using System.Text.RegularExpressions;
using TowelBorrowing.Data;
using TowelBorrowing.Models;
using FuzzySharp;

namespace TowelBorrowing.Services.Impls;

internal class GuestCardService : IGuestCardService
{
	private readonly ILogger<GuestCardService> _logger;
	private readonly IScreenService _screenService;
	private readonly IConfiguration _configuration;
	private readonly IOcrService _ocrService;
	private readonly AppDbContext _dbContext;

	public GuestCardService(
		ILogger<GuestCardService> logger,
		IScreenService screenService,
		IConfiguration configuration,
		IOcrService ocrService,
		AppDbContext dbContext)
	{
		_logger = logger;
		_screenService = screenService;
		_configuration = configuration;
		_ocrService = ocrService;
		_dbContext = dbContext;
	}

	public async Task<GuestCardOcrResult> ExtractGuestCardAsync(int maxRetries = 3)
	{
		var cardResult = new GuestCardOcrResult();

		var imagePath = await CaptureAndSaveGuestCardImageAsync(320, 320);
		if (string.IsNullOrEmpty(imagePath))
		{
			_logger.LogError("Failed to capture guest card image.");
			cardResult.ErrorMessage = "Chụp màn hình thất bại";
			return cardResult;
		}

		int attempt = 0;
		while (attempt < maxRetries)
		{
			attempt++;
			var result = await ExtractGuestCardFromImageAsync(imagePath);
			if (string.IsNullOrEmpty(result.ErrorMessage))
			{
				try
				{
					File.Delete(imagePath);
				}
				catch (Exception)
				{
				}
				return result;
			}

			_logger.LogWarning("OCR attempt {Attempt} failed for {ImagePath}.", attempt, Path.GetFileName(imagePath));
		}

		_logger.LogError("OCR failed after {MaxRetries} attempts for {ImagePath}.", maxRetries, Path.GetFileName(imagePath));
		cardResult.ErrorMessage = "OCR thất bại";
		return cardResult;
	}

	private async Task<string> CaptureAndSaveGuestCardImageAsync(int width, int height)
	{
		try
			{
			// create directory if not exists
			if (!Directory.Exists("imgs"))
				Directory.CreateDirectory("imgs");

			var curDate = DateTime.Now;
			var dirName = Path.Combine("imgs", $"{curDate.ToString("yyyy-MM-dd")}");
			if (!Directory.Exists(dirName))
				Directory.CreateDirectory(dirName);

			var mainAppProcess = (await _dbContext.AppSettings
				.FirstOrDefaultAsync(x => x.Key == Constants.RoomManagementAppName))?.Value ?? ""; 
			var screen = CaptureAroundCenter(mainAppProcess, width, height);
			var imagePath = Path.Combine(dirName, $"screenshot_{Guid.NewGuid().ToString()}.png");
			await File.WriteAllBytesAsync(imagePath, screen);
			return imagePath;
		}
		catch (Exception)
		{
			_logger.LogError("Failed to capture and save guest card image.");
			return string.Empty;
		}
	}

	private async Task<GuestCardOcrResult> ExtractGuestCardFromImageAsync(string imagePath)
	{
		var (text, errorMessage) = await ExtractTextFromImageAsync(imagePath);
		if (string.IsNullOrWhiteSpace(text))
			return new GuestCardOcrResult { ErrorMessage = errorMessage };

		var result = new GuestCardOcrResult();
		result.CardInfo = new GuestCardOcrDto();

		var lines = text.Split('\n')
						.Select(l => l.Trim())
						.Where(l => !string.IsNullOrEmpty(l))
						.ToList();

		// --- Guest Card ---
		result.CardInfo.Card = ExtractField(lines, new[] { "GUEST CARD", "[GUESTCARD]", "IGUESTCARD" }, 8);
		if (result.CardInfo.Card == null)
		{
			result.ErrorMessage = "Không đọc được Guest Card";
			return result;
		}

		// --- Holder Name ---
		result.CardInfo.HolderName = ExtractField(lines, new[] { "HOLDER NAME", "HOLDER", "HOLDERN" }, 4);
		if (result.CardInfo.HolderName == null)
		{
			result.ErrorMessage = "Không đọc được Holder Name";
			return result;
		}

		// --- Room Number ---
		result.CardInfo.RoomNo = ExtractField(lines, new[] { "ROOM NO", "ROOM", "RM" }, 4)
								 ?? result.CardInfo.HolderName; // fallback nếu Room không có

		if (result.CardInfo.RoomNo == null)
		{
			result.ErrorMessage = "Không đọc được Room No";
			return result;
		}

		result.CardInfo.Floor = result.CardInfo.RoomNo?[0].ToString();

		// --- Building ---
		result.CardInfo.Building = ExtractField(lines, new[] { "BLDG", "BIDG", "BLD", "TOA" }, 2)?.TrimStart('0')
								   ?? "1";

		return result;
	}

	private string? ExtractField(IEnumerable<string> lines, string[] labels, int numberLength)
	{
		foreach (var line in lines)
		{
			foreach (var label in labels)
			{
				var idx = line.ToUpper().IndexOf(label.ToUpper());
				if (idx >= 0)
				{
					var remainder = line.Substring(idx + label.Length);
					var match = Regex.Match(remainder, @"\d{" + numberLength + "}");
					if (match.Success)
						return match.Value;
				}
			}
		}
		return null;
	}
	private async Task<(string text, string? errorMessage)> ExtractTextFromImageAsync(string imagePath)
	{
		if (!File.Exists(imagePath))
		{
			_logger.LogError("File {fileName} does not exist.", Path.GetFileName(imagePath));
			return (string.Empty, $"File {Path.GetFileName(imagePath)} does not exist.");
		}

		try
		{
			var ocrText = await _ocrService.RecognizeAsync(imagePath);
			_logger.LogInformation("OCR Result:\n{OcrText}",	 ocrText);
			return (ocrText, null);
		}
		catch (Exception)
		{
			_logger.LogError("Failed to extract text from image {imagePath}.", Path.GetFileName(imagePath));
			return (string.Empty, $"Failed to extract text from image {Path.GetFileName(imagePath)}.");
		}
	}

	private byte[] CaptureAroundCenter(string processName, int width, int height)
	{
		var center = _screenService.GetProcessCenter(processName);
		if (center == null) return Array.Empty<byte>();

		int halfHoz = width / 2;
		int halfVer = height / 2;

		var region = new System.Drawing.Rectangle(
			center.Value.X - halfHoz,
			center.Value.Y - halfVer,
			width,
			height);

		return _screenService.CaptureProcessRegion(processName, region);
	}

}
