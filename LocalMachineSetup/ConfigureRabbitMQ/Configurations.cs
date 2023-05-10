using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureRabbitMQ;

record struct ExchangeConfiguration(string ExchangeName, string[] Queues);
internal static class Configurations
{
    internal const string AnalyticsQueue = "analytics";
    internal const string UpstreamQueue = "upstream";
    public static ExchangeConfiguration AcquitionExchange_Exchange = new ExchangeConfiguration("AcquitionExchange", new string []  { AnalyticsQueue, UpstreamQueue});

    public static ExchangeConfiguration[] ExchangeConfigurations = { AcquitionExchange_Exchange };
}
