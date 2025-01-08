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
        IMachineHistoryHelper _machineHistoryHelper;
        public MqttBackendService(IMqttClient mqttClient, IMqttDataService mqttDataService, IMachineHistoryHelper machineHistoryHelper,
                            IHubContext<MachineDataHub> hubContext, IOptions<MqttSettings> options, ILogger logger)
        {
            _mqttClient = mqttClient;
            _mqttDataService = mqttDataService;
            _hubContext = hubContext;
            _mqttSettings = options.Value;
            _cancellationTokenSource = new CancellationTokenSource();
            _logger = logger;
            _machineHistoryHelper = machineHistoryHelper;

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                try
                {
                    var topic = e.ApplicationMessage.Topic;
                    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                    if (!_machineHistoryHelper.IsValidPayload(payload))
                    {
                        _logger.Warning($"Invalid payload received on topic {topic}: {payload}");
                        return;
                    }

                    _logger.Information($"Subscribed message: {payload}");

                    _logger.Information($"Sending message to the SignalR Clients: {payload}");
                    await _hubContext.Clients.All.SendAsync(_mqttSettings.HubMethodName, payload);

                    _logger.Information($"Saving message to Database: {payload}");
                    await _mqttDataService.SaveMqttMessageAsync(topic, payload, CancellationToken.None);

                }
                catch (Exception ex)
                {
                    _logger.Error($"MQTT Error: {ex.Message}. Payload: {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                }
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
                foreach (var topic in _mqttSettings.Topics)
                {
                    _logger.Information($"Subscribing to topic: {topic}");
                    await _mqttClient.SubscribeAsync(
                        new MqttTopicFilterBuilder().WithTopic(topic).Build(),
                        cancellationToken
                    );
                }
               
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
