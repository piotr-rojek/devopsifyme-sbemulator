using AutoFixture;
using NSubstitute;
using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq.Tests
{
    public class RabbitMqUtilitiesTest : Base
    {
        RabbitMqUtilities _sut => Fixture.Freeze<RabbitMqUtilities>();

        [Theory]
        [InlineData("dummyQueue", "dummyQueue", "", "dummyQueue")]
        [InlineData("topicName", "topicName", "", "topicName")]
        [InlineData("topicName/Subscriptions/subName", "topicName", "", "topicName-sub-subName")]
        public void ThatAddressIsTranslated(string address, string expectedExchange, string expectedRoutingKey, string expectedQueue)
        {
            (string exchange, string queue, string routingKey) = _sut.GetExachangeAndQueue(address);

            Assert.Multiple(
                () => Assert.Equal(expectedQueue, queue),
                () => Assert.Equal(expectedExchange, exchange),
                () => Assert.Equal(expectedRoutingKey, routingKey)
            );
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ThatRabbitObjectAreCreated(bool isSender)
        {
            var address = Fixture.Create<string>();
            IModel channel = Fixture.Create<IModel>();

            _sut.EnsureExists(channel, address, isSender);

            channel.Received(1).ExchangeDeclare(Arg.Any<string>(), "fanout", true, false);
            channel.Received(isSender ? 0 : 1).QueueDeclare(Arg.Any<string>(), true, false, false);
            channel.Received(isSender ? 0 : 1).QueueBind(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }
    }
}