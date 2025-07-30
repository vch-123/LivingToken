using AvaloniaApplication4.Service;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication4.Clients
{
    public class MqttClient
    {
        public IManagedMqttClient? client;
        private readonly ConfigService _configService;
        private readonly EquipDataService _equipDataService;
        private readonly AppSettingsService _appSettingsService;

        public MqttClient()
        {
            _configService = App.Services.GetRequiredService<ConfigService>();
            _equipDataService = App.Services.GetRequiredService<EquipDataService>();
            _appSettingsService = App.Services.GetRequiredService<AppSettingsService>();
            InitializeMQTT();
        }

        private void InitializeMQTT()
        {
            var mqttFactory = new MqttFactory();
            client = mqttFactory.CreateManagedMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(_appSettingsService.BrokerIp, _appSettingsService.Port)
                .WithClientId(_appSettingsService.ClientId)
                .Build();

            var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(mqttClientOptions)
                .Build();

            client.ConnectedAsync += AddTopicSubscription;
            client.ConnectingFailedAsync += LogConnectingFailed;
            client.ApplicationMessageReceivedAsync += Client_ApplicationMessageReceivedAsync;
            client.StartAsync(managedMqttClientOptions); //托管客户端  启动   
            client.ConnectionStateChangedAsync += LogConnectionStateChanged;
        }

        private Task LogConnectionStateChanged(EventArgs arg)
        {
            return Task.CompletedTask;
        }

        private Task LogConnectingFailed(ConnectingFailedEventArgs arg)
        {
            return Task.CompletedTask;
        }


        public Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {

            var content = Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment);

            if (!string.IsNullOrEmpty(content))
            {
                _equipDataService.UpdateCranes(arg.ApplicationMessage.Topic, content);
            }
        
            return Task.CompletedTask;
        }


        private Task AddTopicSubscription(MqttClientConnectedEventArgs arg)
        {

            foreach(var topic in _appSettingsService.Topics)
            {
                client.SubscribeAsync(topic);
            }
                       
            return Task.CompletedTask;
        }
    }
}
