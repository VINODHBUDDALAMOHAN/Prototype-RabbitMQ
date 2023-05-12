using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventApps.Common;

internal static class QueueBinding
{
    //Declare and bind a queue to an exchange
    internal static (string, EventingBasicConsumer) DeclareAndBindQueueToExchange(IModel channel, string qName, string exchangeName)
    {
        //Declare the exchange
        channel.ExchangeDeclare(exchangeName, "fanout", true, false);

        //Declare the queue
        var queueName = channel.QueueDeclare(qName, true, false, false, null).QueueName;

        //Bind the queue to the exchange
        channel.QueueBind(queueName, exchangeName, "");

        //Create a consumer for the queue
        var consumer = new EventingBasicConsumer(channel);

        //Return the queue name and consumer
        return (queueName, consumer);
    }
}
