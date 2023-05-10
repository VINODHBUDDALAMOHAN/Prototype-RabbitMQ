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
    internal static (string, EventingBasicConsumer) DeclareAndBindQueueToExchange(IModel channel, string qName, string exchangeName)
    {
        channel.ExchangeDeclare(exchangeName, "fanout", true, false);
        var queueName = channel.QueueDeclare(qName, true, false, false, null).QueueName;
        channel.QueueBind(queueName, exchangeName, "");
        var consumer = new EventingBasicConsumer(channel);
        return (queueName, consumer);
    }
}
