using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TowelBorrowing.Data;

namespace TowelBorrowing.Services.Impls;

public class BorrowRecordLoggerService : IDisposable
{
	private readonly ILogger<BorrowRecordLoggerService> _logger;
	private readonly IServiceScopeFactory _scopeFactory;
	private Timer? _timer;

	public BorrowRecordLoggerService(
		ILogger<BorrowRecordLoggerService> logger,
		IServiceScopeFactory scopeFactory)
	{
		_logger = logger;
		_scopeFactory = scopeFactory;
	}

	public void Start()
	{
		_logger.LogInformation("BorrowRecordLoggerService started");

		// Log ngay lập tức khi start
		_ = LogBorrowRecordCountAsync();

		// Sau đó log mỗi phút
		_timer = new Timer(
			async _ => await LogBorrowRecordCountAsync(),
			null,
			TimeSpan.FromMinutes(1),
			TimeSpan.FromMinutes(1));
	}

	private async Task LogBorrowRecordCountAsync()
	{
		try
		{
			using var scope = _scopeFactory.CreateScope();
			var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
			await using var dbContext = await dbContextFactory.CreateDbContextAsync();

			// Calculate today's date range in Vietnam timezone
			var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
			var vietnamNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, vietnamTimeZone);
			var todayStartVietnam = vietnamNow.Date;
			var todayStartUtc = TimeZoneInfo.ConvertTimeToUtc(todayStartVietnam, vietnamTimeZone);
			var todayEndUtc = todayStartUtc.AddDays(1);

			// DEBUG: Log thời gian để kiểm tra
			_logger.LogInformation("DEBUG - UTC Now: {UtcNow}", DateTime.UtcNow);
			_logger.LogInformation("DEBUG - Vietnam Now: {VietnamNow}", vietnamNow);
			_logger.LogInformation("DEBUG - Today Start UTC: {TodayStartUtc}", todayStartUtc);
			_logger.LogInformation("DEBUG - Today End UTC: {TodayEndUtc}", todayEndUtc);

			// Count BorrowRecords for today
			var count = await dbContext.BorrowRecords
				.Where(x => x.CreatedAt >= todayStartUtc && x.CreatedAt < todayEndUtc)
				.CountAsync();

			// Log the count with Vietnam time
			_logger.LogInformation(
				"Số lượng BorrowRecord hôm nay ({Date}): {Count} bản ghi",
				vietnamNow.ToString("dd/MM/yyyy HH:mm:ss"),
				count);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while logging BorrowRecord count");
		}
	}

	public void Dispose()
	{
		_timer?.Dispose();
		_logger.LogInformation("BorrowRecordLoggerService stopped");
	}
}
