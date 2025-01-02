using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineTracking.Domain.Interfaces.Application
{
    public interface IMqttDataService
    {
        Task SaveMqttMessageAsync(string topic, string message, CancellationToken cancellationToken);
    }
}
