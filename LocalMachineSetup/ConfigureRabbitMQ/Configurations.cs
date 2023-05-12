using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureRabbitMQ;

record struct ExchangeConfiguration(string ExchangeName, string[] Queues);
internal static class Configurations
{
    // Declare two internal constants for the names of two queues
    internal const string AnalyticsQueue = "analytics";
    internal const string UpstreamQueue = "upstream";
    // Create a new ExchangeConfiguration object called "AcquitionExchange" with two queues: AnalyticsQueue and UpstreamQueue
    public static ExchangeConfiguration AcquitionExchange_Exchange = new ExchangeConfiguration("AcquitionExchange", new string[] { AnalyticsQueue, UpstreamQueue });
    public static ExchangeConfiguration[] ExchangeConfigurations = { AcquitionExchange_Exchange };
}
