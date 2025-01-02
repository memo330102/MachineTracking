using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineTracking.Domain.DTOs
{
    public class MachineHistoryDTO
    {
        public string MachineId { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; } = 0;
        public double ChainMovesPerSecond { get; set; }
        public string ArticleNumber { get; set; }
        public string Topic { get; set; }
        public DateTime DataReceivedTimestamp { get; set; } = DateTime.Now;
    }
}
