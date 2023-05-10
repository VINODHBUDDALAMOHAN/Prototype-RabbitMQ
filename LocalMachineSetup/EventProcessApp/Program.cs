
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
