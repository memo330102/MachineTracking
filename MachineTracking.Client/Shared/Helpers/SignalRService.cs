using Microsoft.AspNetCore.SignalR.Client;
using MachineTracking.Domain.Interfaces.Client;

namespace MachineTracking.Client.Shared.Helpers
{
    public class SignalRService : ISignalRService
    {
        private readonly HubConnection _hubConnection;
        private IConfiguration _configuration;

        public event Action<string> OnMachineDataReceived;

        public SignalRService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var hubURL = $"{_configuration["ApiBaseUrl"]}{_configuration["SignalRSettings:HubName"]}";
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubURL)
                .WithAutomaticReconnect()
                .Build();

            string hubMethodName = _configuration["SignalRSettings:HubMethodName"] ?? "";

            if (!string.IsNullOrEmpty(hubMethodName))
            {
                _hubConnection.On<string>(hubMethodName, data => OnMachineDataReceived?.Invoke(data));
            }
        }

        public async Task StartAsync()
        {
            try
            {
                if (_hubConnection.State == HubConnectionState.Disconnected)
                {
                    await _hubConnection.StartAsync();
                }
            }
            catch (Exception ex)
            {

            }

        }

        public async Task StopAsync()
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.StopAsync();
            }
        }
    }
}
