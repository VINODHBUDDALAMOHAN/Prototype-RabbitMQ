
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
