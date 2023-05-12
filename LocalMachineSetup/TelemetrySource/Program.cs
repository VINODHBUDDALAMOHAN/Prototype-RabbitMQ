
using ConfigureRabbitMQ;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace IoTSensorApplication;

class Program
{
    private static ConnectionFactory? _factory;
    private static IConnection? _connection;
    private static IModel? _model;

    static void Main()
    {
        CreateConnection();
        int i = 0;
        while (i < 10)
        {
            i++;
            SendMessage($$"""{'Id': 'device-client-01', 'msg': '{{i}}-{{Guid.NewGuid()}}'}""");
        }
    }

    private static void CreateConnection()
    {
        _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest", Port = 5672  };
        _connection = _factory.CreateConnection();
        _model = _connection.CreateModel();
        //_model.ExchangeDeclare(Configurations.AcquitionExchange_Exchange.ExchangeName, "fanout", true, false);
    }

    private static void SendMessage(string message)
    {
        var basicProperties = _model.CreateBasicProperties();
        basicProperties.Persistent = true;
        _model.BasicPublish(Configurations.AcquitionExchange_Exchange.ExchangeName, "", basicProperties, Encoding.UTF8.GetBytes(message));
        Console.WriteLine($"Sent {message}");
    }

}

//MQTTCODE
//using System;
//using System.Text;
//using MQTTnet;
//using MQTTnet.Client;
//using MQTTnet.Client.Options;

//namespace IoTSensorApplication
//{
//    class Program
//    {
//        static async System.Threading.Tasks.Task Main()
//        {
//            var factory = new MqttFactory();
//            var mqttClient = factory.CreateMqttClient();

//            var options = new MqttClientOptionsBuilder()
//                .WithTcpServer("localhost", 1883)
//                .WithCredentials("guest", "guest")
//                .Build();

//            await mqttClient.ConnectAsync(options);

//            int i = 0;
//            while (i < 10)
//            {
//                i++;
//                SendMessage(mqttClient, $"{{'Id': 'device-client-01', 'msg': '{i}-{Guid.NewGuid()}'}}");
//            }

//            Console.ReadKey();

//            await mqttClient.DisconnectAsync();
//        }

//        private static void SendMessage(IMqttClient mqttClient, string message)
//        {
//            var messagePayload = new MqttApplicationMessageBuilder()
//                .WithTopic(Configurations.AcquitionExchange_Exchange.ExchangeName)
//                .WithPayload(message)
//                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
//                .Build();

//            mqttClient.PublishAsync(messagePayload).Wait();

//            Console.WriteLine($"Sent {message}");
//        }
//    }
//}
