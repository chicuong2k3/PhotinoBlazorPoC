using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace TowelBorrowing.Services.Impls;

internal class OcrService : IOcrService
{
	private readonly HttpClient _client;
	private readonly IConfiguration _configuration;

	public OcrService(IConfiguration configuration)
	{
		_client = new HttpClient();
		_configuration = configuration;
	}

	public async Task<string> RecognizeAsync(string imagePath)
	{
		using var form = new MultipartFormDataContent();

		var fileBytes = await File.ReadAllBytesAsync(imagePath);
		var fileContent = new ByteArrayContent(fileBytes);
		fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

		form.Add(fileContent, "file", Path.GetFileName(imagePath));

		var apiKey = _configuration["OcrService:ApiKey"]
			?? throw new ArgumentNullException("OcrService:ApiKey is not configured");

		_client.DefaultRequestHeaders.Remove("X-API-KEY");
		_client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

		var apiUrl = _configuration["OcrService:ApiBaseUrl"]
			?? throw new ArgumentNullException("OcrService:ApiBaseUrl is not configured");
		var response = await _client.PostAsync($"{apiUrl.Trim("/")}/ocr", form);

		var json = await response.Content.ReadAsStringAsync();

		return json;
	}
}
