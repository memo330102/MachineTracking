using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineTracking.Domain.DTOs
{
    public class MachineHistoryGetRequestDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public string SearchText { get; set; } = string.Empty;
        public string MachineId { get; set; } = string.Empty;
    }
}
