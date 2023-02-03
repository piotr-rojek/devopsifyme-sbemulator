using Amqp;
using Amqp.Framing;
using Amqp.Types;
using AutoFixture;
using NSubstitute;
using RabbitMQ.Client;
using ServiceBusEmulator.Abstractions.Azure;
using ServiceBusEmulator.RabbitMq.Commands;

namespace ServiceBusEmulator.RabbitMq.Tests.Commands
{
    public class PeekMessageCommandTest : Base<PeekMessageCommand>
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 5)]
        [InlineData(3, 5)]
        [InlineData(5, 5)]
        public void ThatPeeksMessages(int givenSequence, int givenCount)
        {
            string givenQueue = Fixture.Create<string>();

            IModel channel = Fixture.Freeze<IModel>();
            var request = new Dictionary<string, object>
            {
                { ManagementConstants.Properties.FromSequenceNumber, givenSequence },
                { ManagementConstants.Properties.MessageCount, givenCount }
            };

            (var response, var status) = Sut.Handle(new Message(request.ToMap()), channel, givenQueue);

            Assert.Equal(AmqpResponseStatusCode.OK, status);
            Assert.IsType<AmqpValue>(response.BodySection);

            var responseMap = ((AmqpValue)response.BodySection).Value as Map;
            Assert.NotNull(responseMap);

            var responseList = responseMap[ManagementConstants.Properties.Messages] as List;
            Assert.NotNull(responseList);

            Assert.Equal(responseList.Count, givenCount);
            channel.Received(givenSequence + givenCount - 1).BasicGet(Arg.Any<string>(), false);
            channel.Received(givenSequence + givenCount - 1).BasicReject(Arg.Any<ulong>(), true);
        }
    }
}
