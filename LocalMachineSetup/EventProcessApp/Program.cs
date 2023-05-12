
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
                var (queueName, _consumer) = QueueBinding.DeclareAndBindQueueToExchange(channel, Configurations.AnalyticsQueue, Configurations.AcquitionExchange_Exchange.ExchangeName);

                // Print the queue name
                Console.WriteLine($"Listening at {queueName}");
                // Add an event handler for when a message is received
                _consumer!.Received += (model, ea) =>
                {
                    // Print the received message
                    Console.WriteLine($"Received {Encoding.UTF8.GetString(ea.Body.ToArray())}");
                    // Sleep for 1 second
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    // Print the processed message
                    Console.WriteLine($"Processed {Encoding.UTF8.GetString(ea.Body.ToArray())}");
                };

                // Consume the queue with the given consumer
                channel.BasicConsume(queueName, true, _consumer);
                // Wait for user input
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

