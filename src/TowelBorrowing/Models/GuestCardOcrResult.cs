

namespace TowelBorrowing.Models;

public class GuestCardOcrResult
{
	public GuestCardOcrDto CardInfo { get; set; } = default!;
	public string? ErrorMessage { get; set; }
}

public class GuestCardOcrDto
{
	public string Card { get; set; } = string.Empty;
	public string HolderName { get; set; } = string.Empty;
	public string Building { get; set; } = string.Empty;
	public string Floor { get; set; } = string.Empty;
	public string RoomNo { get; set; } = string.Empty;

	public override string ToString()
	{
		return $"Card: {Card}; HolderName: {HolderName}; Building: {Building}; Floor: {Floor}; RoomNo: {RoomNo}";
	}
}
