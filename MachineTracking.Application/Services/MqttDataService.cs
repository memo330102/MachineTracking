using MachineTracking.Domain.DTOs;
using MachineTracking.Domain.Interfaces.Application;
using MachineTracking.Domain.Interfaces.Infrastructure;
using System.Text.Json;

namespace MachineTracking.Application.Services
{
    public class MqttDataService : IMqttDataService
    {
        private readonly IMachineHistoryRepository _machineHistoryRepository;
        public MqttDataService(IMachineHistoryRepository machineHistoryRepository)
        {
            _machineHistoryRepository = machineHistoryRepository;
        }

        public async Task SaveMqttMessageAsync(string topic, string message, CancellationToken cancellationToken)
        {
            try
            {
                var machineHistory = JsonSerializer.Deserialize<MachineHistoryDTO>(message);

                await _machineHistoryRepository.AddAsync(machineHistory);

                Console.WriteLine($"Message received on topic '{topic}': {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }

}
