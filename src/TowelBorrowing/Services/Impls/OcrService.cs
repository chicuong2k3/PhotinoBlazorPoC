using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using TowelBorrowing.Data;

namespace TowelBorrowing.Services.Impls;

internal class OcrService : IOcrService
{
	private readonly HttpClient _client;
	private readonly AppDbContext _dbContext;

	public OcrService(AppDbContext dbContext)
	{
		_client = new HttpClient();
		_dbContext = dbContext;
	}

	public async Task<string> RecognizeAsync(string imagePath)
	{
		using var form = new MultipartFormDataContent();

		var fileBytes = await File.ReadAllBytesAsync(imagePath);
		var fileContent = new ByteArrayContent(fileBytes);
		fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

		form.Add(fileContent, "file", Path.GetFileName(imagePath));

		var apiKey = (await _dbContext.AppSettings.FirstOrDefaultAsync(x => x.Key == Constants.OcrApiKey))?.Value
			?? "my-secret-api-key";

		_client.DefaultRequestHeaders.Remove("X-API-KEY");
		_client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

		var apiUrl = (await _dbContext.AppSettings.FirstOrDefaultAsync(x => x.Key == Constants.OcrApiUrl))?.Value
			?? "http://localhost:10000";
		var response = await _client.PostAsync($"{apiUrl.Trim("/")}/ocr", form);

		var json = await response.Content.ReadAsStringAsync();

		return json;
	}
}
