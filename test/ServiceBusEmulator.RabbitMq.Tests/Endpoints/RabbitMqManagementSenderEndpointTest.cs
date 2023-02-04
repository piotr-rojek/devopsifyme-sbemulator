using ServiceBusEmulator.RabbitMq.Endpoints;
using AutoFixture;
using RabbitMQ.Client;
using Amqp.Framing;
using Amqp;
using ServiceBusEmulator.Abstractions.Azure;

namespace ServiceBusEmulator.RabbitMq.Tests.Links
{
    public class RabbitMqManagementSenderEndpointTest : Base<RabbitMqManagementSenderEndpoint>
    {
        public RabbitMqManagementSenderEndpointTest()
        {
            Fixture.RegisterAmqpDummyTypes();
            Fixture.CustomizeAmqpMessage();
        }

        [Fact]
        public void ThatResponseContainsBasicFields()
        {
            var message = Fixture.Create<Message>();
            var channel = Fixture.Create<IModel>();
            var target = Fixture.Create<Target>();
            var expectedStatus = AmqpResponseStatusCode.Accepted;
            Fixture.Inject(expectedStatus);

            Sut.SetContext(channel, target);
            var response = Sut.GetResponse(message);

            Assert.Multiple(
                () => Assert.Equal(response.ApplicationProperties[ManagementConstants.Response.StatusCode], (int)expectedStatus),
                () => Assert.Equal(response.ApplicationProperties[ManagementConstants.Response.StatusDescription], expectedStatus.ToString()),

                () => Assert.Equal(response.Properties.CorrelationId, message.Properties.MessageId)
            );
        }
    }
}