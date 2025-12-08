using Microsoft.EntityFrameworkCore;
using TowelBorrowing.Data;

namespace TowelBorrowing.Services.Impls;

public class DatabaseService : IDatabaseService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public DatabaseService(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<DatabaseStorageInfo> GetDatabaseStorageInfoAsync()
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            
            var result = await dbContext.Database
                .SqlQueryRaw<DatabaseSizeRaw>(
                    "SELECT " +
                    "pg_size_pretty(pg_database_size(current_database())) as \"UsedSize\", " +
                    "pg_database_size(current_database()) as \"UsedBytes\"")
                .FirstOrDefaultAsync();

            if (result == null)
                return GetDefaultInfo();

            long totalBytes = 3221225472; // 3GB
            long usedBytes = result.UsedBytes;
            long remainingBytes = Math.Max(0, totalBytes - usedBytes);

            var usagePercentage = (double)usedBytes / totalBytes * 100;

            return new DatabaseStorageInfo
            {
                UsedSize = FormatBytes(usedBytes),
                TotalSize = FormatBytes(totalBytes),
                RemainingSize = FormatBytes(remainingBytes),
                UsagePercentage = Math.Round(usagePercentage, 2)
            };
        }
        catch
        {
            return GetDefaultInfo();
        }
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes == 0) return "0 B";
        const long unit = 1024;
        var units = new[] { "B", "KB", "MB", "GB", "TB" };
        int unitIndex = 0;
        double size = bytes;
        while (size >= unit && unitIndex < units.Length - 1)
        {
            size /= unit;
            unitIndex++;
        }
        return $"{size:F2} {units[unitIndex]}";
    }

    private static DatabaseStorageInfo GetDefaultInfo() => new()
    {
        UsedSize = "N/A",
        TotalSize = "N/A",
        RemainingSize = "N/A",
        UsagePercentage = 0
    };

    private class DatabaseSizeRaw
    {
        public string? UsedSize { get; set; }
        public long UsedBytes { get; set; }
    }
}
