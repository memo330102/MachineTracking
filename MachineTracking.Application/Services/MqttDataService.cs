using MachineTracking.Domain.DTOs;
using MachineTracking.Domain.Enums;
using MachineTracking.Domain.Interfaces.Application;
using MachineTracking.Domain.Interfaces.Infrastructure;
using Serilog;
using System.Text.Json;

namespace MachineTracking.Application.Services
{
    public class MqttDataService : IMqttDataService
    {
        private readonly IMachineHistoryRepository _machineHistoryRepository;
        private readonly ILogger _logger;
        public MqttDataService(IMachineHistoryRepository machineHistoryRepository, ILogger logger)
        {
            _machineHistoryRepository = machineHistoryRepository;
            _logger = logger;
        }

        public async Task SaveMqttMessageAsync(string topic, string message, CancellationToken cancellationToken)
        {
            try
            {
                var machineHistory = JsonSerializer.Deserialize<MachineHistoryDTO>(message);

                if (machineHistory == null)
                {
                    _logger.Error($"MQTT Error : Invalid message received on topic '{topic}': {message}");
                    return;
                }

                machineHistory.Topic = topic;
                machineHistory.StatusId = MachineHistoryHelper.MapStatusToStatusId(machineHistory);

                await _machineHistoryRepository.AddAsync(machineHistory);
                _logger.Information($"Message processed and saved for topic '{topic}': {message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"MQTT Error : Unexpected error while receiving message on topic'{topic}': {message} : {ex}");
            }
        }
    }

}
