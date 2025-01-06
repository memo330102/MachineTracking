using MachineTracking.Domain.DTOs.MachineHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineTracking.Domain.Interfaces.Infrastructure
{
    public interface IMachineHistoryRepository
    {
        Task<IEnumerable<MachineHistoryDTO>> GetAllAsync();
        Task<MachineHistoryDTO?> GetByIdAsync(int id);
        Task AddAsync(MachineHistoryDTO entity);
        Task<IEnumerable<MachineHistoryDTO>> GetLastestDataOfAllMachinesAsync();
        Task<IEnumerable<MachineHistoryDTO>> GetMachineHistoriesAsync(string machineId);
    }
}
