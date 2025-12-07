namespace TowelBorrowing.Services;

public interface IBorrowNotificationService
{
    event Func<BorrowNotification, Task>? OnBorrowUpdated;
    event Func<BorrowNotification, Task>? OnReturnUpdated;
    event Func<BorrowRecordNotification, Task>? OnBorrowRecordUpdated;

    Task NotifyBorrowAsync(string cardNumber, string roomNo, string building, int quantity);
    Task NotifyReturnAsync(string cardNumber, string roomNo, string building, int quantity);
    Task NotifyBorrowRecordUpdatedAsync(string cardNumber, int borrowedCount, int returnedCount);
}

public class BorrowNotification
{
    public string CardNumber { get; set; } = string.Empty;
    public string RoomNo { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime Timestamp { get; set; }
}

public class BorrowRecordNotification
{
    public string CardNumber { get; set; } = string.Empty;
    public int BorrowedCount { get; set; }
    public int ReturnedCount { get; set; }
    public int RemainingCount { get; set; }
    public DateTime Timestamp { get; set; }
}
