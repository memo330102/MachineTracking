namespace MachineTracking.Domain.DTOs.MachineHistory
{
    public class MachineHistoryPaginatedResponse
    {
        public int TotalCount { get; set; }
        public IEnumerable<MachineHistoryDTO> MachineHistories { get; set; }
    }
}
