
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

    private const string ExchangeName = "PublishSubscribe_Exchange";

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
        _model.ExchangeDeclare(ExchangeName, "fanout", true, false);
    }

    private static void SendMessage(string message)
    {
        var basicProperties = _model.CreateBasicProperties();
        basicProperties.Persistent = true;
        _model.BasicPublish(ExchangeName, "", basicProperties, Encoding.UTF8.GetBytes(message));
        Console.WriteLine($"Sent {message}");
    }

}
