
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using EventApps.Common;
using ConfigureRabbitMQ;

namespace EventProcessApp;

class Program
{
    private static ConnectionFactory? _factory;
    private static IConnection? _connection;
    private static EventingBasicConsumer? _consumer;

    static void Main()
    {
        _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
        using (_connection = _factory.CreateConnection())
        {
            using (var channel = _connection.CreateModel())
            {
                var (queueName, _consumer) = QueueBinding.DeclareAndBindQueueToExchange(channel, Configurations.AnalyticsQueue, Configurations.AcquitionExchange_Exchange.ExchangeName);

                Console.WriteLine($"Listening at {queueName}");
                _consumer!.Received += (model, ea) =>
                {
                    Console.WriteLine($"Received {Encoding.UTF8.GetString(ea.Body.ToArray())}");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    Console.WriteLine($"Processed {Encoding.UTF8.GetString(ea.Body.ToArray())}");
                };

                channel.BasicConsume(queueName, true, _consumer);
                Console.ReadKey();
            }
        }
    }
}
//COde for MQTT

//using MQTTnet;
//using MQTTnet.Client;
//using MQTTnet.Client.Options;
//using MQTTnet.Client.Subscribing;
//using MQTTnet.Extensions.ManagedClient;
//using System;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace EventProcessApp
//{
//    class Program
//    {
//        static async Task Main()
//        {
//            var options = new MqttClientOptionsBuilder()
//                .WithTcpServer("localhost", 1883)
//                .WithCredentials("guest", "guest")
//                .Build();

//            var factory = new MqttFactory();
//            var mqttClient = factory.CreateMqttClient();

//            var topics = new TopicFilterBuilder().WithTopic(Configurations.AnalyticsQueue).Build();
//            var subscriptions = new MqttClientSubscribeOptionsBuilder()
//                .WithTopicFilter(topics)
//                .Build();

//            mqttClient.UseConnectedHandler(async e =>
//            {
//                Console.WriteLine($"Listening at {Configurations.AnalyticsQueue}");
//                await mqttClient.SubscribeAsync(subscriptions);
//            });

//            mqttClient.UseApplicationMessageReceivedHandler(e =>
//            {
//                Console.WriteLine($"Received {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
//                Thread.Sleep(TimeSpan.FromSeconds(1));
//                Console.WriteLine($"Processed {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
//            });

//            await mqttClient.ConnectAsync(options);
//            Console.ReadKey();

//            await mqttClient.DisconnectAsync();
//        }
//    }
//}

