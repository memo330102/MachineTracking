using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineTracking.Domain.DTOs.MQTT
{
    public class MqttSettings
    {
        public string Topic { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string TcpServer { get; set; } = string.Empty;
        public string HubMethodName { get; set; }  = string.Empty;
    }
}
