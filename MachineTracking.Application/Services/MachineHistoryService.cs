using MachineTracking.Domain.DTOs;
using MachineTracking.Domain.Interfaces.Application;
using MachineTracking.Domain.Interfaces.Infrastructure;

namespace MachineTracking.Application.Services
{
    public class MachineHistoryService : IMachineHistoryService
    {
        private readonly IMachineHistoryRepository _machineHistoryRepository;

        public MachineHistoryService(IMachineHistoryRepository machineHistoryRepository)
        {
            _machineHistoryRepository = machineHistoryRepository;
        }
        public async Task AddAsync(MachineHistoryDTO entity)
        {
            await _machineHistoryRepository.AddAsync(entity);
        }

        public async Task<IEnumerable<MachineHistoryDTO>> GetAllAsync()
        {
            return await _machineHistoryRepository.GetAllAsync();
        }

        public async Task<MachineHistoryDTO?> GetByIdAsync(int id)
        {
            return await _machineHistoryRepository.GetByIdAsync(id);
        }
        public async Task<MachineHistoryPaginatedResponse> GetMachineHistoriesAsync(MachineHistoryGetRequestDTO request)
        {
            var response = await _machineHistoryRepository.GetMachineHistoriesAsync(request.MachineId);

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                response = response.Where(item =>
                           item.ArticleNumber.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                           item.DataReceivedTimestamp.ToString().Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                           item.ChainMovesPerSecond.ToString().Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                           item.Status.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = response.Count();

            var skip = (request.PageNumber - 1) * request.PageSize;
            var take = request.PageSize;

            response = response.Skip(skip).Take(take);

            return new MachineHistoryPaginatedResponse()
            {
                TotalCount = totalCount,
                MachineHistories = response
            };
        }
        public async Task<IEnumerable<MachineHistoryDTO>> GetLastestDataOfAllMachinesAsync()
        {
            return await _machineHistoryRepository.GetLastestDataOfAllMachinesAsync();
        }
    }

}
