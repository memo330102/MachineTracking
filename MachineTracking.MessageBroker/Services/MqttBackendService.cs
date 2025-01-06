using MachineTracking.Domain.Interfaces.Application;
using MQTTnet;
using MQTTnet.Client;
using System.Text;
using Microsoft.Extensions.Hosting;
using MachineTracking.MessageBroker.Hubs;
using Microsoft.AspNetCore.SignalR;
using MachineTracking.Domain.DTOs.MQTT;
using Microsoft.Extensions.Options;
using Serilog;

namespace MachineTracking.MessageBroker.Services
{
    public class MqttBackendService : IHostedService
    {
        private readonly IMqttClient _mqttClient;
        private readonly IMqttDataService _mqttDataService;
        private readonly IHubContext<MachineDataHub> _hubContext;
        private readonly MqttSettings _mqttSettings;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ILogger _logger;
        public MqttBackendService(IMqttClient mqttClient, IMqttDataService mqttDataService, 
                            IHubContext<MachineDataHub> hubContext, IOptions<MqttSettings> options, ILogger logger)
        {
            _mqttClient = mqttClient;
            _mqttDataService = mqttDataService;
            _hubContext = hubContext;
            _mqttSettings = options.Value;
            _cancellationTokenSource = new CancellationTokenSource();
            _logger = logger;

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                _logger.Information($"Subscribed message: {payload}");

                _logger.Information($"Saving message to Database: {payload}");
                await _mqttDataService.SaveMqttMessageAsync(topic, payload, CancellationToken.None);

                _logger.Information($"Sending message to the SignalR: {payload}");
                await _hubContext.Clients.All.SendAsync(_mqttSettings.HubMethodName, payload);
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                var mqttOptions = new MqttClientOptionsBuilder()
                    .WithClientId(_mqttSettings.ClientId)
                    .WithTcpServer(_mqttSettings.TcpServer) 
                    .Build();

                _logger.Information($"Connecting to MQTT broker...");
                await _mqttClient.ConnectAsync(mqttOptions, cancellationToken);

                _logger.Information($"Subscribing to topic: {_mqttSettings.Topic}");
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(_mqttSettings.Topic).Build(), cancellationToken);
                _logger.Information("Listening for MQTT messages...");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in MQTT background service: {ex.Message}");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource?.Cancel();

            if (_mqttClient.IsConnected)
            {
                _logger.Information("Disconnecting from MQTT broker...");
                await _mqttClient.DisconnectAsync(new MqttClientDisconnectOptions(), cancellationToken);
                _logger.Information("MQTT client disconnected.");
            }
        }
    }

}
