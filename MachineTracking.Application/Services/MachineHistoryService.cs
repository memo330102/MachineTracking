using MachineTracking.Domain.DTOs.MachineHistory;
using MachineTracking.Domain.Interfaces.Application;
using MachineTracking.Domain.Interfaces.Infrastructure;
using Serilog;

namespace MachineTracking.Application.Services
{
    public class MachineHistoryService : IMachineHistoryService
    {
        private readonly IMachineHistoryRepository _machineHistoryRepository;
        private readonly ILogger _logger;
        public MachineHistoryService(IMachineHistoryRepository machineHistoryRepository, ILogger logger)
        {
            _machineHistoryRepository = machineHistoryRepository;
            _logger = logger;
        }
        public async Task AddAsync(MachineHistoryDTO entity)
        { 
            try
            {
                await _machineHistoryRepository.AddAsync(entity);
            }
            catch(Exception ex)
            {
                _logger.Error($"Unexpected error while saving message to database  : {ex}");
            }
        }

        public async Task<IEnumerable<MachineHistoryDTO>> GetAllAsync()
        {
            try
            {
                var response = await _machineHistoryRepository.GetAllAsync();
                return response;
            }
            catch(Exception ex)
            {
                _logger.Error($"Unexpected error while retrieving data from database  : {ex}");
                return Enumerable.Empty<MachineHistoryDTO>();
            }
        }

        public async Task<MachineHistoryDTO?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _machineHistoryRepository.GetByIdAsync(id);
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error while retrieving data from database  : {ex}");
                return new MachineHistoryDTO();
            }
        }
        public async Task<MachineHistoryPaginatedResponse> GetMachineHistoriesAsync(MachineHistoryGetRequestDTO request)
        {
            try
            {
                DateTime now = DateTime.Now;

                var response = await _machineHistoryRepository.GetMachineHistoriesAsync(request.MachineId);

                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    response = response.Where(item =>
                               item.ArticleNumber.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                               item.DataReceivedTimestamp.ToString().Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                               item.ChainMovesPerSecond.ToString().Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                               item.Status.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase));
                }
                int totalCount = 0;
                if (request.PageNumber != 0)
                {
                    totalCount = response.Count();

                    var skip = (request.PageNumber - 1) * request.PageSize;
                    var take = request.PageSize;

                    response = response.Skip(skip).Take(take);
                }

                if (request.Time != TimeSpan.Zero)
                {
                    response = response.Where(entry => now - entry.DataReceivedTimestamp <= request.Time);
                }

                return new MachineHistoryPaginatedResponse()
                {
                    TotalCount = totalCount,
                    MachineHistories = response
                };
            }
            catch(Exception ex)
            {
                _logger.Error($"Unexpected error while retrieving data from database  : {ex}");
                return new MachineHistoryPaginatedResponse()
                {
                    TotalCount = 0,
                    MachineHistories = Enumerable.Empty<MachineHistoryDTO>()
                };
            }

        }
        public async Task<IEnumerable<MachineHistoryDTO>> GetLastestDataOfAllMachinesAsync()
        {
            try
            {
                var response = await _machineHistoryRepository.GetLastestDataOfAllMachinesAsync();
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error while retrieving data from database  : {ex}");
                return Enumerable.Empty<MachineHistoryDTO>();
            }
        }
    }

}
