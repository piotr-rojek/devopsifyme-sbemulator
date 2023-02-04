using Amqp.Listener;
using ServiceBusEmulator.RabbitMq.Endpoints;
using AutoFixture;
using RabbitMQ.Client;
using Amqp.Framing;
using NSubstitute;

namespace ServiceBusEmulator.RabbitMq.Tests.Links
{
    public class RabbitMqSenderEndpointTest : Base<RabbitMqSenderEndpoint>
    {
        public RabbitMqSenderEndpointTest()
        {
            Fixture.RegisterAmqpDummyTypes();
            Fixture.CustomizeAmqpMessage();
        }

        [Fact]
        public void ThatPublishesMessage()
        {
            var messageContext = Fixture.Create<MessageContext>();
            var channel = Fixture.Create<IModel>();
            var target = Fixture.Create<Target>();

            Sut.SetContext(channel, target);
            Sut.OnMessage(messageContext);

            channel.Received(1).BasicPublish(Arg.Any<string>(), Arg.Any<string>(), true, Arg.Any<IBasicProperties>(), Arg.Any<ReadOnlyMemory<byte>>());
        }
    }
}