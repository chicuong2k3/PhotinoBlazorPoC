namespace TowelBorrowing.Services;

public interface IBorrowNotificationService
{
    Task NotifyBorrowAsync(string cardNumber, string roomNo, string building, int quantity);
    Task NotifyReturnAsync(string cardNumber, string roomNo, string building, int quantity);
    Task NotifyBorrowRecordUpdatedAsync(string cardNumber, int borrowedCount, int returnedCount);
}
