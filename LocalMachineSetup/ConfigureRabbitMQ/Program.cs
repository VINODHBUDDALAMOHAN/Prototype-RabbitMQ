﻿
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
