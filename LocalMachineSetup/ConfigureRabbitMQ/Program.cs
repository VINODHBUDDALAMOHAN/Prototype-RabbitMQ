
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
        CreateLocalMachineConfiguration();
        CreateServerConfiguration();
    }

    private static void CreateLocalMachineConfiguration()
    {
        // TODO pull out all hardcoded values in json files
        // TODO password shall be provided as command line parameters or vault
        var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest", Port = 5672 };
        var connection = factory.CreateConnection();
        var model = connection.CreateModel();

        foreach (var exchange in Configurations.ExchangeConfigurations)
        {
            // Create exchange
            model.ExchangeDeclare(exchange.ExchangeName, "fanout", true, false);
            Console.WriteLine($"Created Exchange {exchange.ExchangeName}");

            // Create all queues and exchange bindings
            foreach (var q in exchange.Queues)
            {
                var queueName = model.QueueDeclare(q, true, false, false, null).QueueName;
                model.QueueBind(queueName, exchange.ExchangeName, "");
                Console.WriteLine($"Created Queue  {exchange.ExchangeName}:{q}");
            }
        }
    }

    private static void CreateServerConfiguration()
    {
        // TODO pull out all hardcoded values in json files
        // TODO password shall be provided as command line parameters or vault
        var factory = new ConnectionFactory { HostName = "vm-az220-training-gw0002-vinuaz2202.eastus.cloudapp.azure.com", UserName = "guest", Password = "guest", Port = 5672 };
        var connection = factory.CreateConnection();
        var model = connection.CreateModel();

        foreach (var exchange in Configurations.ExchangeConfigurations)
        {
            // Create exchange
            model.ExchangeDeclare(exchange.ExchangeName, "fanout", true, false);
            Console.WriteLine($"Created Exchange {exchange.ExchangeName}");

            // Create all queues and exchange bindings
            foreach (var q in exchange.Queues)
            {
                var queueName = model.QueueDeclare(q, true, false, false, null).QueueName;
                model.QueueBind(queueName, exchange.ExchangeName, "");
                Console.WriteLine($"Created Queue  {exchange.ExchangeName}:{q}");
            }
        }
    }

}
//MQTTCODE

//using MQTTnet;
//using MQTTnet.Client;
//using MQTTnet.Client.Connecting;
//using MQTTnet.Client.Options;
//using MQTTnet.Client.Subscribing;
//using MQTTnet.Protocol;
//using System;
//using System.Threading.Tasks;

//namespace ConfigureMQTT
//{
//    class Program
//    {
//        static async Task Main(string[] args)
//        {
//            await CreateLocalMachineConfiguration();
//            await CreateServerConfiguration();
//        }

//        private static async Task CreateLocalMachineConfiguration()
//        {
//            // TODO pull out all hardcoded values in json files
//            // TODO password shall be provided as command line parameters or vault
//            var options = new MqttClientOptionsBuilder()
//                .WithTcpServer("localhost", 1883)
//                .WithCredentials("guest", "guest")
//                .Build();

//            var factory = new MqttFactory();
//            var client = factory.CreateMqttClient();

//            await client.ConnectAsync(options);

//            foreach (var exchange in Configurations.ExchangeConfigurations)
//            {
//                // Create exchange
//                await client.SubscribeAsync(new MqttTopicFilterBuilder()
//                    .WithTopic(exchange.ExchangeName)
//                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
//                    .Build());

//                Console.WriteLine($"Created Exchange {exchange.ExchangeName}");

//                // Create all queues and exchange bindings
//                foreach (var q in exchange.Queues)
//                {
//                    await client.SubscribeAsync(new MqttTopicFilterBuilder()
//                        .WithTopic($"{exchange.ExchangeName}/{q}")
//                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
//                        .Build());

//                    Console.WriteLine($"Created Queue  {exchange.ExchangeName}:{q}");
//                }
//            }
//        }

//        private static async Task CreateServerConfiguration()
//        {
//            // TODO pull out all hardcoded values in json files
//            // TODO password shall be provided as command line parameters or vault
//            var options = new MqttClientOptionsBuilder()
//                .WithTcpServer("vm-az220-training-gw0002-vinuaz2202.eastus.cloudapp.azure.com", 1883)
//                .WithCredentials("guest", "guest")
//                .Build();

//            var factory = new MqttFactory();
//            var client = factory.CreateMqttClient();

//            await client.ConnectAsync(options);

//            foreach (var exchange in Configurations.ExchangeConfigurations)
//            {
//                // Create exchange
//                await client.SubscribeAsync(new MqttTopicFilterBuilder()
//                    .WithTopic(exchange.ExchangeName)
//                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
//                    .Build());

//                Console.WriteLine($"Created Exchange {exchange.ExchangeName}");

//                // Create all queues and exchange bindings
//                foreach (var q in exchange.Queues)
//                {
//                    await client.SubscribeAsync(new MqttTopicFilterBuilder()
//                        .WithTopic($"{exchange.ExchangeName}/{q}")
//                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
//                        .Build());

//                    Console.WriteLine($"Created Queue  {exchange.ExchangeName}:{q}");
//                }
//            }
//        }
//    }
//}

