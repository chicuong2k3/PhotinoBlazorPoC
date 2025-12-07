namespace TowelBorrowing.Services;

public interface IDatabaseService
{
    Task<DatabaseStorageInfo> GetDatabaseStorageInfoAsync();
}

public class DatabaseStorageInfo
{
    public string UsedSize { get; set; } = string.Empty;
    public string TotalSize { get; set; } = string.Empty;
    public string RemainingSize { get; set; } = string.Empty;
    public double UsagePercentage { get; set; }
}
