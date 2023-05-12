
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
    private static bool _useMqtt = true;
    //This code creates a connection and sends a message 10 times.
    static async Task Main()
    {
        Action<string> sendMessage;
        if (!_useMqtt)
        {
            var model = CreateConnection();
            sendMessage = (msg) => { SendMessage(model, msg); };
        }
        else
        {
            var mqttClient = await CreateMqttConnection();
            sendMessage = (msg) => { SendMqttMessage(mqttClient, msg).Wait(); };
        }

        //Initialize a counter
        int i = 0;
        //Loop 10 times
        while (i < 10)
        {
            //Increment the counter
            i++;
            //Send a message with the counter and a new GUID
            sendMessage($$"""{'Id': 'device-client-01', 'msg': '{{i}}-{{Guid.NewGuid()}}'}""");
        }
    }
    //This code creates a connection to a RabbitMQ server.
    private static IModel CreateConnection()
    {
        //Create a new ConnectionFactory object with the necessary parameters
        var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest", Port = 5672 };

        //Create a connection using the ConnectionFactory
        var connection = factory.CreateConnection();

        //Create a model using the connection
        var model = connection.CreateModel();

        return model;
    }


    //This code sends a message to a RabbitMQ exchange
    private static void SendMessage(IModel model, string message)
    {
        //Create a basic properties object
        var basicProperties = model.CreateBasicProperties();
        //Set the message to be persistent
        basicProperties.Persistent = true;
        //Publish the message to the exchange
        model.BasicPublish(Configurations.AcquitionExchange_Exchange.ExchangeName, "", basicProperties, Encoding.UTF8.GetBytes(message));
        //Print out the message that was sent
        Console.WriteLine($"Sent {message}");
    }

    private static async Task<IMqttClient> CreateMqttConnection()
    {
        // Create a connection factory
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 1883,
            UserName = "guest",
            Password = "guest",
            ClientProvidedName = "device-client-01"
        };

        // Create an MQTT client
        var mqttFactory = new MqttFactory();
        var mqttClient = mqttFactory.CreateMqttClient();

        // Create an options object for the MQTT client
        var mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(factory.HostName, factory.Port)
            .WithCredentials(factory.UserName, factory.Password)
            .WithClientId(factory.ClientProvidedName)
            .WithCleanSession()
            .Build();

        await mqttClient.ConnectAsync(mqttOptions);

        return mqttClient;
    }
    private static async Task SendMqttMessage(IMqttClient mqttClient, string message)
    {
        var messagePayload = new MqttApplicationMessageBuilder()
            .WithTopic(Configurations.AcquitionExchange_Exchange.ExchangeName)
            .WithPayload(message)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await mqttClient.PublishAsync(messagePayload);

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
