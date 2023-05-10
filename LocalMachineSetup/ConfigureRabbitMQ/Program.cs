
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;
using System.Xml.Linq;

namespace ConfigureRabbitMQ;

class Program
{
    private static ConnectionFactory? _factory;
    private static IConnection? _connection;
    private static IModel? _model;
    static void Main()
    {
        CreateConnection();
    }

    private static void CreateConnection()
    {
        // TODO pull out all hardcoded values in json files
        // TODO password shall be provided as command line parameters or vault
        _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest", Port = 5672 };
        _connection = _factory.CreateConnection();
        _model = _connection.CreateModel();

        foreach (var exchange in Configurations.ExchangeConfigurations)
        {
            // Create exchange
            _model.ExchangeDeclare(exchange.ExchangeName, "fanout", true, false);
            Console.WriteLine($"Created Exchange {exchange.ExchangeName}");

            // Create all queues and exchange bindings
            foreach (var q in exchange.Queues)
            {
                var queueName = _model.QueueDeclare(q, true, false, false, null).QueueName;
                _model.QueueBind(queueName, exchange.ExchangeName, "");
                Console.WriteLine($"Created Queue  {exchange.ExchangeName}:{q}");
            }
        }
    }

}
