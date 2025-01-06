using MachineTracking.Domain.DTOs.MachineHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineTracking.Domain.Interfaces.Application
{
    public interface IMachineHistoryService
    {
        Task<IEnumerable<MachineHistoryDTO>> GetAllAsync();
        Task<MachineHistoryDTO?> GetByIdAsync(int id);
        Task<MachineHistoryPaginatedResponse> GetMachineHistoriesAsync(MachineHistoryGetRequestDTO machineHistoryGetRequestDTO);
        Task AddAsync(MachineHistoryDTO entity);
        Task<IEnumerable<MachineHistoryDTO>> GetLastestDataOfAllMachinesAsync();
    }
}
