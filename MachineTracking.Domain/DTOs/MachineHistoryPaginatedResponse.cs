using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineTracking.Domain.DTOs
{
    public class MachineHistoryPaginatedResponse
    {
        public int TotalCount { get; set; }
        public IEnumerable<MachineHistoryDTO> MachineHistories { get; set; }
    }
}
