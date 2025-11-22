namespace TowelBorrowing.Data.Models;

public class GuestCard
{
	public string CardNumber { get; set; } = string.Empty;
	public string HolderName { get; set; } = string.Empty;
	public string Building { get; set; } = string.Empty;
	public string Floor { get; set; } = string.Empty;
	public string RoomNo { get; set; } = string.Empty;
	public ICollection<BorrowRecord> Logs { get; set; } = new List<BorrowRecord>();

	public GuestCard() { }
}
