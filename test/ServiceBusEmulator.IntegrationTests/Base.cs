using AutoFixture;
using Azure.Messaging.ServiceBus;

namespace ServiceBusEmulator.IntegrationTests
{
    public class Base
    {
        private const string ConnectionString = "Endpoint=sb://sbemulator/;SharedAccessKeyName=all;SharedAccessKey=CLwo3FQ3S39Z4pFOQDefaiUd1dSsli4XOAj3Y9Uh1E=;EnableAmqpLinkRedirect=false";

        public Base()
        {
            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpTcp
            };

            Client = new ServiceBusClient(ConnectionString, clientOptions);

            Fixture = new Fixture();
        }

        protected IFixture Fixture { get; private set; }

        protected ServiceBusClient Client { get; private set; }
    }
}
