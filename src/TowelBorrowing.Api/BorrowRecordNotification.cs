namespace TowelBorrowing.Api;

public class BorrowRecordNotification
{
	public string CardNumber { get; set; } = string.Empty;
	public int BorrowedCount { get; set; }
	public int ReturnedCount { get; set; }
	public int RemainingCount { get; set; }
	public DateTime Timestamp { get; set; }
}
