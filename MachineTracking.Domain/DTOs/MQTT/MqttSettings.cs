using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineTracking.Domain.DTOs.MQTT
{
    public class MqttSettings
    {
        public string[] Topics { get; set; } 
        public string ClientId { get; set; } = string.Empty;
        public string TcpServerHost { get; set; } = string.Empty;
        public int? TcpServerPort { get; set; } 
        public string HubMethodName { get; set; }  = string.Empty;
    }
}
