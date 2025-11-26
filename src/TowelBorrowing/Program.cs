using Hedin.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino.Blazor;
using Photino.Blazor.CustomWindow.Extensions;
using System.Reflection;
using TowelBorrowing.Data;
using TowelBorrowing.Services;
using TowelBorrowing.Services.Impls;

namespace TowelBorrowing;

public class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

		var config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();

		appBuilder.Services.AddSingleton<IConfiguration>(config);

		appBuilder.Services.AddLogging(builder =>
		{
			builder.SetMinimumLevel(LogLevel.Information);
			builder.AddConsole();
		});
		appBuilder.Services.AddUIConfiguration(config);

		appBuilder.RootComponents.Add<App>("app");

		appBuilder.Services.AddCustomWindow();

		appBuilder.Services.AddDbContext<AppDbContext>(options =>
			options.UseSqlite($"Data Source={Constants.DatabasePath}"));

		appBuilder.Services.AddScoped<IGuestCardService, GuestCardService>();
		appBuilder.Services.AddScoped<IOcrService, OcrService>();
		appBuilder.Services.AddScoped<IImportService, ImportService>();
		appBuilder.Services.AddScoped<IExportService, ExportService>();
		appBuilder.Services.AddSingleton<IScreenService>(provider =>
		{
			var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

			if (OperatingSystem.IsWindows())
				return new ScreenServiceWindows(loggerFactory.CreateLogger<ScreenServiceWindows>());

			//if (OperatingSystem.IsLinux())
			//	return new ScreenCaptureLinux(loggerFactory.CreateLogger<ScreenCaptureLinux>());

			if (OperatingSystem.IsMacOS())
				return new ScreenServiceMacOS(loggerFactory.CreateLogger<ScreenServiceMacOS>());

			throw new PlatformNotSupportedException();
		});

		var app = appBuilder.Build();

		try
		{
			var iconPath = Path.Combine(AppContext.BaseDirectory, "favicon.ico");

			if (!File.Exists(iconPath))
			{
				using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TowelBorrowing.favicon.ico");
				using var fs = new FileStream(iconPath, FileMode.Create, FileAccess.Write);
				stream?.CopyTo(fs);
			}

			app.MainWindow
			.SetSize(640, 480)
			.SetDevToolsEnabled(true)
			.SetLogVerbosity(0)
			.SetResizable(false)
			.SetChromeless(true)
			.SetIconFile(iconPath);

			AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
			{
				app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
			};

			app.Services.SeedData();
			app.Run();
		}
		catch (Exception ex)
		{
			var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
			Directory.CreateDirectory(logDir); 
			var logFile = Path.Combine(logDir, "crashlog.txt");
			File.WriteAllText(logFile, ex.ToString());
		}
		
		
	}
}

