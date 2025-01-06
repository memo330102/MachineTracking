using MachineTracking.Domain.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace MachineTracking.Client.Shared.Helpers
{
    public class SignalRService
    {
        private readonly HubConnection _hubConnection;
        public event Action<string> OnMachineDataReceived;

        public SignalRService()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7100/machinedatahub")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string>("ReceiveMachineData", data =>
            {
                OnMachineDataReceived?.Invoke(data);
            });
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
