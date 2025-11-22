using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using System.Text.RegularExpressions;
using TowelBorrowing.Models;

namespace TowelBorrowing.Services.Impls;

internal class GuestCardService : IGuestCardService
{
	private readonly ILogger<GuestCardService> _logger;
	private readonly IScreenService _screenService;
	private readonly IConfiguration _configuration;
	private readonly IOcrService _ocrService;

	public GuestCardService(
		ILogger<GuestCardService> logger,
		IScreenService screenService,
		IConfiguration configuration,
		IOcrService ocrService)
	{
		_logger = logger;
		_screenService = screenService;
		_configuration = configuration;
		_ocrService = ocrService;
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

			var mainAppProcess = _configuration["MainAppProcessName"] ??
				throw new InvalidOperationException("MainAppProcessName is not configured.");
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
		{
			return new() { ErrorMessage = errorMessage };
		}

		var result = new GuestCardOcrResult();
		result.CardInfo = new GuestCardOcrDto();

		var textProcessed = text.Replace(" ", "").Replace(".", "").Replace(",", "").Replace(":", "").ToUpper();

		// Guest Card
		var matchGuestCard = Regex.Match(textProcessed, @"\[GUESTCARD\](\d+)");
		if (matchGuestCard.Success)
			result.CardInfo.Card = matchGuestCard.Groups[1].Value;
		else
		{
			result.ErrorMessage = "Thiếu Guest Card";
			return result;
		}

		// Holder Name
		var matchHolder = Regex.Match(textProcessed, @"HOLDERNAME(\d{4})|HOLDERN(\d{4})");
		if (matchHolder.Success)
			result.CardInfo.HolderName = matchHolder.Groups[1].Success
				? matchHolder.Groups[1].Value : matchHolder.Groups[2].Value;
		else
		{
			result.ErrorMessage = "Thiếu Holder Name";
			return result;
		}

		// Room
		string? roomNo = null;
		var matchRoom = Regex.Match(textProcessed, @"ROOMNO(\d{4})|ROOMS?(\d{4})");
		if (matchRoom.Success)
		{
			roomNo = matchRoom.Groups[1].Success ?
				matchRoom.Groups[1].Value : matchRoom.Groups[2].Value;
			roomNo = roomNo.TrimStart('0');
		}
		else
		{
			result.ErrorMessage = "Thiếu Room Number";
			return result;
		}

		if (string.IsNullOrEmpty(roomNo) && !string.IsNullOrEmpty(result.CardInfo.HolderName))
			roomNo = result.CardInfo.HolderName;

		if (string.IsNullOrEmpty(roomNo))
		{
			var allNumbers = Regex.Matches(textProcessed, @"\d{4}").Select(m => m.Value);
			roomNo = allNumbers.FirstOrDefault(n => n[0] >= '1' && n[0] <= '9');
		}

		if (!string.IsNullOrEmpty(roomNo))
		{
			result.CardInfo.RoomNo = roomNo; result.CardInfo.Floor = roomNo[0].ToString();
		}
		string bldgNo = "1";
		var matchBldg = Regex.Match(textProcessed, @"(BIDG|BLDG|BLD|TOA)(\d{1,2})");
		if (matchBldg.Success)
		{
			bldgNo = matchBldg.Groups[2].Value.TrimStart('0');
		}	
		result.CardInfo.Building = bldgNo;

		_logger.LogInformation("Extracted Guest Card Info: {@CardInfo}", result.CardInfo);

		return result;
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
			return (string.Join("\n", ocrText), null);
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
