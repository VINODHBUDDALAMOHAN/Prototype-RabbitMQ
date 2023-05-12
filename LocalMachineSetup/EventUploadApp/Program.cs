
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

    // Rewritten code with comments

    static void Main()
    {
        // Create a new connection factory with the given hostname, username, and password
        _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
        // Create a new connection using the connection factory
        using (_connection = _factory.CreateConnection())
        {
            // Create a new channel using the connection
            using (var channel = _connection.CreateModel())
            {
                // Declare and bind a queue to an exchange using the given configurations
                var (queueName, _consumer) = QueueBinding.DeclareAndBindQueueToExchange(channel, Configurations.UpstreamQueue, Configurations.AcquitionExchange_Exchange.ExchangeName);

                // Set the acknowledgement to true
                var ack = true;
                // Print out the queue name
                Console.WriteLine($"Listening at {queueName}");
                // When a message is received, print out the message, wait for 1 second, and then acknowledge the message
                _consumer!.Received += (model, ea) =>
                {
                    Console.WriteLine($"Received {Encoding.UTF8.GetString(ea.Body.ToArray())}");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    channel.BasicNack(ea.DeliveryTag, ack, !ack);
                    Console.WriteLine($"Upload {ack} {Encoding.UTF8.GetString(ea.Body.ToArray())}");
                    ack = !ack;
                };

                // Consume the queue
                channel.BasicConsume(queueName, false, _consumer);
                // Wait for user input
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
