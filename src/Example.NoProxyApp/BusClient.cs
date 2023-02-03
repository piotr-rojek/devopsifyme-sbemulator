using Azure.Messaging.ServiceBus;

namespace Example.NoProxyApp
{
    internal class BusClient
    {
        private const string ConnectionString = "Endpoint=sb://localhost/;SharedAccessKeyName=all;SharedAccessKey=CLwo3FQ3S39Z4pFOQDefaiUd1dSsli4XOAj3Y9Uh1E=;EnableAmqpLinkRedirect=false";

        public BusClient()
        {
            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpTcp
            };

            Client = new ServiceBusClient(ConnectionString, clientOptions);
        }

        private ServiceBusClient Client { get; }

        public async Task SendMessageAsync(string queueName)
        {
            var sender = Client.CreateSender(queueName);

            var message = new ServiceBusMessage("body body")
            {
                ContentType = "text/plain",
                CorrelationId = "correlatiionid",
                MessageId = "messageid",
                PartitionKey = "partitionkey",
                ReplyTo = "replyto",
                ReplyToSessionId = "replytosessionid",
                ScheduledEnqueueTime = DateTime.Now.AddYears(1),
                SessionId = "sessionid",
                Subject = "subject",
                TimeToLive = TimeSpan.FromSeconds(666),
                To = "out-to",
                TransactionPartitionKey = "partitionkey"
            };

            message.ApplicationProperties["testapp1"] = "value1";
            message.ApplicationProperties["testapp2"] = "value2";
            message.ApplicationProperties["testapp3"] = "value3";

            await sender.SendMessageAsync(message);
        }

        public async Task<ServiceBusReceivedMessage> ReceiveMessageAsync(string queueName)
        {
            var receiver = Client.CreateReceiver(queueName, new ServiceBusReceiverOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
            });

            return await receiver.ReceiveMessageAsync();
        }
    }
}
