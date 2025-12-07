namespace TowelBorrowing.Api;

using Microsoft.AspNetCore.SignalR;
public class BorrowHub : Hub
{
	private readonly ILogger<BorrowHub> _logger;

	public BorrowHub(ILogger<BorrowHub> logger)
	{
		_logger = logger;
	}

	public override async Task OnConnectedAsync()
	{
		_logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		_logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
		await base.OnDisconnectedAsync(exception);
	}
	public async Task NotifyBorrow(string cardNumber, string roomNo, string building, int quantity)
	{
		var notification = new BorrowNotification
		{
			CardNumber = cardNumber,
			RoomNo = roomNo,
			Building = building,
			Quantity = quantity,
			Timestamp = DateTime.UtcNow
		};

		_logger.LogInformation("Borrow notification: {@Notification}", notification);

		await Clients.All.SendAsync("BorrowUpdated", notification);
	}

	public async Task NotifyReturn(string cardNumber, string roomNo, string building, int quantity)
	{
		var notification = new BorrowNotification
		{
			CardNumber = cardNumber,
			RoomNo = roomNo,
			Building = building,
			Quantity = quantity,
			Timestamp = DateTime.UtcNow
		};

		_logger.LogInformation("Return notification: {@Notification}", notification);

		await Clients.All.SendAsync("ReturnUpdated", notification);
	}

	public async Task NotifyBorrowRecordUpdated(string cardNumber, int borrowedCount, int returnedCount)
	{
		var notification = new BorrowRecordNotification
		{
			CardNumber = cardNumber,
			BorrowedCount = borrowedCount,
			ReturnedCount = returnedCount,
			RemainingCount = borrowedCount - returnedCount,
			Timestamp = DateTime.UtcNow
		};

		_logger.LogInformation("Borrow record updated: {@Notification}", notification);

		await Clients.All.SendAsync("BorrowRecordUpdated", notification);
	}
}
