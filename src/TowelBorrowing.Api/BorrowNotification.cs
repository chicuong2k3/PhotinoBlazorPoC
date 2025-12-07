namespace TowelBorrowing.Api;

public class BorrowNotification
{
	public string CardNumber { get; set; } = string.Empty;
	public string RoomNo { get; set; } = string.Empty;
	public string Building { get; set; } = string.Empty;
	public int Quantity { get; set; }
	public DateTime Timestamp { get; set; }
}
