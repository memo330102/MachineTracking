using MachineTracking.Domain.Interfaces.Application;
using MQTTnet;
using MQTTnet.Client;
using System.Text;
using Microsoft.Extensions.Hosting;
using MachineTracking.Domain.DTOs;
using Newtonsoft.Json;
using MachineTracking.MessageBroker.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MachineTracking.MessageBroker.Services
{
    public class MqttBackendService : IHostedService
    {
        private readonly IMqttClient _mqttClient;
        private readonly IMqttDataService _mqttDataService;
        private readonly IHubContext<MachineDataHub> _hubContext;

        private const string Topic = "gotecgroup/machine";
        private CancellationTokenSource _cancellationTokenSource;

        public MqttBackendService(IMqttClient mqttClient, IMqttDataService mqttDataService, IHubContext<MachineDataHub> hubContext)
        {
            _mqttClient = mqttClient;
            _mqttDataService = mqttDataService;
            _hubContext = hubContext;

            // Attach message handler
            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                await _mqttDataService.SaveMqttMessageAsync(topic, payload, CancellationToken.None);

                await _hubContext.Clients.All.SendAsync("ReceiveMachineData", payload);
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                // Configure the client options
                var mqttOptions = new MqttClientOptionsBuilder()
                    .WithClientId("BackendService")
                    .WithTcpServer("localhost") // Replace with your broker address
                    .Build();
                // Connect to the MQTT broker
                Console.WriteLine("Connecting to MQTT broker...");
                await _mqttClient.ConnectAsync(mqttOptions, cancellationToken);

                // Subscribe to the topic
                Console.WriteLine($"Subscribing to topic: {Topic}");
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(Topic).Build(), cancellationToken);

                Console.WriteLine("Listening for MQTT messages...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MQTT background service: {ex.Message}");
            }

            // Keep the service running
            //await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource?.Cancel();

            // Disconnect the MQTT client asynchronously
            if (_mqttClient.IsConnected)
            {
                Console.WriteLine("Disconnecting from MQTT broker...");
                await _mqttClient.DisconnectAsync(new MqttClientDisconnectOptions(), cancellationToken);
                Console.WriteLine("MQTT client disconnected.");
            }
        }
    }

}
