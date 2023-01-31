using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Example.AzureFunction
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        [ServiceBusOutput("mytopic", ServiceBusEntityType.Topic, Connection = "EmulatorConnectionString")]
        public string Run([ServiceBusTrigger("myqueue", Connection = "EmulatorConnectionString")] string myQueueItem)
        {
            _logger.LogInformation($"C# ServiceBus QUEUE trigger function processed message: {myQueueItem}");

            return $"processing of '{myQueueItem}' item finished";
        }

        [Function("Function2")]
        public void Run2([ServiceBusTrigger("mytopic", "subscription", Connection = "EmulatorConnectionString")] string myQueueItem)
        {
            _logger.LogInformation($"C# ServiceBus SUBSCRIPTION ONE #1 trigger function processed message: {myQueueItem}");
        }

        [Function("Function3")]
        public void Run3([ServiceBusTrigger("mytopic", "subscription2", Connection = "EmulatorConnectionString")] string myQueueItem)
        {
            _logger.LogInformation($"C# ServiceBus SUBSCRIPTION TWO #2 trigger function processed message: {myQueueItem}");
        }
    }
}
