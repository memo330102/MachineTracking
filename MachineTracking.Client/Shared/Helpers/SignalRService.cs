using Microsoft.AspNetCore.SignalR.Client;
using MachineTracking.Domain.Interfaces.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MachineTracking.Client.Shared.Helpers
{
    public class SignalRService : ISignalRService
    {
        private readonly HubConnection _hubConnection;
        private IConfiguration _configuration;
        private readonly ILogger<SignalRService> _logger;

        public event Action<string> OnMachineDataReceived;

        public SignalRService(IConfiguration configuration, ILogger<SignalRService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var hubURL = $"{_configuration["ApiBaseUrl"]}{_configuration["SignalRSettings:HubName"]}";
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubURL)
                .WithAutomaticReconnect()
                .Build();

            string hubMethodName = _configuration["SignalRSettings:HubMethodName"] ?? "";

            if (!string.IsNullOrEmpty(hubMethodName))
            {
                _hubConnection.On<string>(hubMethodName, data => OnMachineDataReceived?.Invoke(data));
                _hubConnection.On<string>(hubMethodName, data =>
                {
                    _logger.LogInformation($"Data received from SignalR: {data}");
                    OnMachineDataReceived?.Invoke(data);
                });
            }
            _logger.LogInformation($"SignalRService initialized with Hub URL: {hubURL}");
        }

        public async Task StartAsync()
        {

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _hubConnection.StartAsync();
                    _logger.LogInformation("SignalR connection started successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to start SignalR connection.");
                }
            }
            else
            {
                _logger.LogWarning($"SignalR connection is already in state: {_hubConnection.State}");
            }
        }

        public async Task StopAsync()
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.StopAsync();
                    _logger.LogInformation($"SignalR connection stopped successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to stop SignalR connection.");
                }
            }
            else
            {
                _logger.LogWarning($"SignalR connection is not in a connected state. Current state: {_hubConnection.State}");
            }
        }
    }
}
