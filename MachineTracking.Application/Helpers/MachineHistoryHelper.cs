using MachineTracking.Domain.DTOs.MachineHistory;
using MachineTracking.Domain.Enums;
using MachineTracking.Domain.Interfaces.Application;
using Serilog;

namespace MachineTracking.Application.Helpers
{
    public class MachineHistoryHelper : IMachineHistoryHelper
    {
        private readonly ILogger _logger;

        public MachineHistoryHelper(ILogger logger)
        {
            _logger = logger;
        }
        public bool IsValidPayload(string payload)
        {
            if (string.IsNullOrEmpty(payload))
            {
                _logger.Warning("Payload is empty or null.");
                return false;
            }
            if (!payload.StartsWith("{") || !payload.EndsWith("}"))
            {
                _logger.Warning("Payload is not a valid JSON.");
                return false;
            }
            return true;
        }

        public int MapStatusToStatusId(MachineHistoryDTO machineHistory)
        {
            return machineHistory.Status switch
            {
                "EngineOn" when machineHistory.ChainMovesPerSecond > 0 => (int)MachineStatusTypeEnum.Active,
                "EngineOn" when machineHistory.ChainMovesPerSecond == 0 => (int)MachineStatusTypeEnum.Idle,
                "EngineOff" when machineHistory.ChainMovesPerSecond <= 0 => (int)MachineStatusTypeEnum.Inactive,
                _ => (int)MachineStatusTypeEnum.Unexpected
            };
        }
    }
}
