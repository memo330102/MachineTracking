
namespace MachineTracking.Domain.Interfaces.Client
{
    public interface ISignalRService
    {
        event Action<string> OnMachineDataReceived;
        Task StartAsync();
        Task StopAsync();
    }
}
