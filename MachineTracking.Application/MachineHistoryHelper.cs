using MachineTracking.Domain.DTOs;
using MachineTracking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineTracking.Application
{
    public static class MachineHistoryHelper
    {
        public static int MapStatusToStatusId(MachineHistoryDTO machineHistory)
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
