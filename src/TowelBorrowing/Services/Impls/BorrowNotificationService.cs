using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TowelBorrowing.Services.Impls;

public class BorrowNotificationService : IBorrowNotificationService
{
    private readonly HubConnection? _hubConnection;
    private readonly ILogger<BorrowNotificationService> _logger;

    public BorrowNotificationService(ILogger<BorrowNotificationService> logger, IConfiguration configuration)
    {
        _logger = logger;

        var hubUrl = configuration.GetValue<string>("SignalRHubUrl") ?? "http://localhost:5000/borrowhub";

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        RegisterEventHandlers();
    }

    private void RegisterEventHandlers()
    {
        if (_hubConnection == null) return;

        _hubConnection.On<dynamic>("BorrowUpdated", (data) =>
        {
            _logger.LogInformation("Borrow updated: {Data}", (object)data);
        });

        _hubConnection.On<dynamic>("ReturnUpdated", (data) =>
        {
            _logger.LogInformation("Return updated: {Data}", (object)data);
        });

        _hubConnection.On<dynamic>("BorrowRecordUpdated", (data) =>
        {
            _logger.LogInformation("Borrow record updated: {Data}", (object)data);
        });

        _hubConnection.Closed += async (exception) =>
        {
            _logger.LogWarning(exception, "SignalR connection closed");
            await Task.CompletedTask;
        };
    }

    public async Task StartAsync()
    {
        try
        {
            if (_hubConnection?.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
                _logger.LogInformation("SignalR connection started");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start SignalR connection");
        }
    }

    public async Task StopAsync()
    {
        try
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.StopAsync();
                _logger.LogInformation("SignalR connection stopped");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop SignalR connection");
        }
    }

    public async Task NotifyBorrowAsync(string cardNumber, string roomNo, string building, int quantity)
    {
        try
        {
            await StartAsync();
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("NotifyBorrow", cardNumber, roomNo, building, quantity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to notify borrow");
        }
    }

    public async Task NotifyReturnAsync(string cardNumber, string roomNo, string building, int quantity)
    {
        try
        {
            await StartAsync();
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("NotifyReturn", cardNumber, roomNo, building, quantity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to notify return");
        }
    }

    public async Task NotifyBorrowRecordUpdatedAsync(string cardNumber, int borrowedCount, int returnedCount)
    {
        try
        {
            await StartAsync();
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("NotifyBorrowRecordUpdated", cardNumber, borrowedCount, returnedCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to notify borrow record updated");
        }
    }
}
