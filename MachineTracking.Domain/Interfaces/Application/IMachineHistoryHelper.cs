using MachineTracking.Domain.DTOs.MachineHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineTracking.Domain.Interfaces.Application
{
    public interface IMachineHistoryHelper
    {
        int MapStatusToStatusId(MachineHistoryDTO machineHistory);
        bool IsValidPayload(string payload);
    }
}
