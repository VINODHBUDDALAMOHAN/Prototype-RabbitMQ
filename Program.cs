using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using RabbitMQ.Client;

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

// Connect to the MQTT broker
var connectResult = await mqttClient.ConnectAsync(mqttOptions);

// Use the MQTT client to interact with RabbitMQ
int count = 0;
while (true)
{
    count++;
    if (count > 10)
        break;

    // Publish a message to a topic
    var message = new MqttApplicationMessageBuilder()
        .WithTopic("mqtt.topic")
        .WithPayload("{'Id': 'device-client-01', 'msg': 'msg'}")
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .Build();

    await mqttClient.PublishAsync(message);
    Thread.Sleep(TimeSpan.FromSeconds(1));

}
Console.Read();
