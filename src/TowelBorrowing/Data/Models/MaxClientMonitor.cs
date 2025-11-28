namespace TowelBorrowing.Data.Models;

public class MaxClientMonitor
{
	public Guid Id { get; private set; }
	public string Building { get; private set; } = string.Empty;
	public string RoomNo { get; private set; } = string.Empty;
	public int MaxQuantity { get; set; }

	private MaxClientMonitor()
	{
		
	}

	public MaxClientMonitor(string building,
						 string roomNo,
						 int maxQuantity)
	{
		Id = Guid.NewGuid();
		Building = building;
		RoomNo = roomNo;
		MaxQuantity = maxQuantity;
	}
}
