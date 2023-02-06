using AutoFixture;
using NSubstitute;
using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq.Tests
{
    public class RabbitMqUtilitiesTest : Base<RabbitMqUtilities>
    {
        [Theory]
       [InlineData("dummyQueue", "dummyQueue", "dummyQueue", "dummyQueue")]
        [InlineData("dummyQueue/$deadletterqueue", "dummyQueue", "dummyQueue-dlq", "dummyQueue-dlq")]
        [InlineData("topicName", "topicName", "topicName", "topicName")]
        [InlineData("topicName/Subscriptions/subName", "topicName", "topicName", "topicName-sub-subName")]
        [InlineData("topicName/Subscriptions/subName/$deadletterqueue", "topicName", "topicName-sub-subName-dlq", "topicName-sub-subName-dlq")]
        public void ThatAddressIsTranslated(string address, string expectedExchange, string expectedRoutingKey, string expectedQueue)
        {
            (string exchange, string queue, string routingKey) = Sut.GetExachangeAndQueueNames(address);

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

            Sut.EnsureExists(channel, address, isSender);

            channel.Received(1).ExchangeDeclare(Arg.Any<string>(), "topic", true, false);
            channel.Received(isSender ? 0 : 2).QueueDeclare(Arg.Any<string>(), true, false, false, Arg.Any<IDictionary<string, object>>());
            channel.Received(isSender ? 0 : 2).QueueBind(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }
    }
}