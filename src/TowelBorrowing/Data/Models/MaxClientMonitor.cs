namespace TowelBorrowing.Data.Models;

public class MaxClientMonitor
{
	public Guid Id { get; private set; }
	public string Building { get; private set; } = string.Empty;
	public string RoomNo { get; private set; } = string.Empty;
	public int MaxQuantity { get; private set; }
	public DateTime Date { get; private set; }

	private MaxClientMonitor()
	{
		
	}

	public MaxClientMonitor(string building,
						 string roomNo,
						 int maxQuantity,
						 DateTime date)
	{
		Id = Guid.NewGuid();
		Building = building;
		RoomNo = roomNo;
		MaxQuantity = maxQuantity;
		Date = date;
	}
}
