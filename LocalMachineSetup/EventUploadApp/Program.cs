
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using EventApps.Common;
using ConfigureRabbitMQ;

namespace EventUploadApp;

class Program
{
    private static ConnectionFactory? _factory;
    private static IConnection? _connection;
    private static EventingBasicConsumer? _consumer;

    private const string ExchangeName = "PublishSubscribe_Exchange";

    static void Main()
    {
        _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
        using (_connection = _factory.CreateConnection())
        {
            using (var channel = _connection.CreateModel())
            {
                var (queueName, _consumer) = QueueBinding.DeclareAndBindQueueToExchange(channel, Configurations.UpstreamQueue, Configurations.AcquitionExchange_Exchange.ExchangeName);

                var ack = true;
                Console.WriteLine($"Listening at {queueName}");
                _consumer!.Received += (model, ea) =>
                {
                    Console.WriteLine($"Received {Encoding.UTF8.GetString(ea.Body.ToArray())}");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    channel.BasicNack(ea.DeliveryTag, ack, !ack);
                    Console.WriteLine($"Upload {ack} {Encoding.UTF8.GetString(ea.Body.ToArray())}");
                    ack = !ack;
                };

                channel.BasicConsume(queueName, false, _consumer);
                Console.ReadKey();
            }
        }
    }
}
// MQTT CODE

//using MQTTnet;
//using MQTTnet.Client;
//using MQTTnet.Client.Options;
//using MQTTnet.Client.Subscribing;
//using MQTTnet.Extensions.ManagedClient;
//using System;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using EventApps.Common;
//using ConfigureMQTT;

//namespace EventUploadApp
//{
//    class Program
//    {
//        static async Task Main()
//        {
//            var factory = new MqttFactory();
//            var client = factory.CreateMqttClient();

//            var options = new MqttClientOptionsBuilder()
//                .WithTcpServer("localhost")
//                .WithCredentials("guest", "guest")
//                .Build();

//            await client.ConnectAsync(options);

//            var topic = Configurations.UpstreamQueue;
//            await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());

//            Console.WriteLine($"Listening at {topic}");

//            bool ack = true;

//            client.UseApplicationMessageReceivedHandler(e =>
//            {
//                Console.WriteLine($"Received {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
//                Thread.Sleep(TimeSpan.FromSeconds(1));

//                var message = new MqttApplicationMessageBuilder()
//                    .WithTopic(topic)
//                    .WithPayload(Encoding.UTF8.GetBytes($"Upload {ack} {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}"))
//                    .WithExactlyOnceQoS()
//                    .Build();

//                client.PublishAsync(message, CancellationToken.None).Wait();
//                Console.WriteLine($"Upload {ack} {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");

//                ack = !ack;
//            });

//            Console.ReadKey();

//            await client.DisconnectAsync();
//        }
//    }
//}
