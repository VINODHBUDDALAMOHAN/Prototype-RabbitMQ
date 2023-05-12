
using ConfigureRabbitMQ;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet;
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

    //This code creates a connection and sends a message 10 times.
    static void Main()
    {
        //Create a connection
        CreateConnection();
        //Initialize a counter
        int i = 0;
        //Loop 10 times
        while (i < 10)
        {
            //Increment the counter
            i++;
            //Send a message with the counter and a new GUID
            SendMessage($$"""{'Id': 'device-client-01', 'msg': '{{i}}-{{Guid.NewGuid()}}'}""");
        }
    }
    //This code creates a connection to a RabbitMQ server.
    private static void CreateConnection()
    {
        //Create a new ConnectionFactory object with the necessary parameters
        _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest", Port = 5672 };

        //Create a connection using the ConnectionFactory
        _connection = _factory.CreateConnection();

        //Create a model using the connection
        _model = _connection.CreateModel();
    }

    //This code sends a message to a RabbitMQ exchange
    private static void SendMessage(string message)
    {
        //Create a basic properties object
        var basicProperties = _model.CreateBasicProperties();
        //Set the message to be persistent
        basicProperties.Persistent = true;
        //Publish the message to the exchange
        _model.BasicPublish(Configurations.AcquitionExchange_Exchange.ExchangeName, "", basicProperties, Encoding.UTF8.GetBytes(message));
        //Print out the message that was sent
        Console.WriteLine($"Sent {message}");
    }

}

//MQTTCODE
//using System;
//using System.Text;
//using MQTTnet;
//using MQTTnet.Client;
//using MQTTnet.Client.Options;

//namespace IoTSensorApplication
//{
//    class Program
//    {
//        static async System.Threading.Tasks.Task Main()
//        {
//            var factory = new MqttFactory();
//            var mqttClient = factory.CreateMqttClient();

//            var options = new MqttClientOptionsBuilder()
//                .WithTcpServer("localhost", 1883)
//                .WithCredentials("guest", "guest")
//                .Build();

//            await mqttClient.ConnectAsync(options);

//            int i = 0;
//            while (i < 10)
//            {
//                i++;
//                SendMessage(mqttClient, $"{{'Id': 'device-client-01', 'msg': '{i}-{Guid.NewGuid()}'}}");
//            }

//            Console.ReadKey();

//            await mqttClient.DisconnectAsync();
//        }

//        private static void SendMessage(IMqttClient mqttClient, string message)
//        {
//            var messagePayload = new MqttApplicationMessageBuilder()
//                .WithTopic(Configurations.AcquitionExchange_Exchange.ExchangeName)
//                .WithPayload(message)
//                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
//                .Build();

//            mqttClient.PublishAsync(messagePayload).Wait();

//            Console.WriteLine($"Sent {message}");
//        }
//    }
//}
