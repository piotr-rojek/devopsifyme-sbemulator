using Amqp.Listener;
using ServiceBusEmulator.RabbitMq.Endpoints;
using AutoFixture;
using RabbitMQ.Client;
using Amqp.Framing;
using NSubstitute;

namespace ServiceBusEmulator.RabbitMq.Tests.Links
{
    public class RabbitMqReceiverEndpointTest : Base<RabbitMqReceiverEndpoint>
    {
        public RabbitMqReceiverEndpointTest()
        {
            Fixture.RegisterAmqpDummyTypes();
        }

        [Theory]
        [MemberData(nameof(Dispositions))]
        public void ThatHandlesDispostion(DeliveryState deliveryState)
        {
            var expectedDeliveryTag = Fixture.Create<ulong>();

            Fixture.CustomizeAmqpMessage(deliveryState: deliveryState);
            Fixture.Inject(deliveryState);
            Fixture.Inject(false);

            var channel = Fixture.Create<IModel>();
            var source = Fixture.Build<Source>().Without(x => x.Outcomes).Without(x => x.DefaultOutcome).Create();
            var context = Fixture.Create<DispositionContext>();
            var deliveryTagTracker = Fixture.Freeze<IRabbitMqDeliveryTagTracker>();
            deliveryTagTracker[Arg.Any<byte[]>()].Returns(expectedDeliveryTag);

            Sut.SetContext(channel, source, ReceiverSettleMode.Second);
            Sut.OnDisposition(context);

            switch (deliveryState)
            {
                case Accepted:
                    channel.Received(1).BasicAck(expectedDeliveryTag, false);
                    break;
                case Released:
                    channel.Received(1).BasicNack(expectedDeliveryTag, false, true);
                    break;
                case Modified:
                    channel.Received(1).BasicNack(expectedDeliveryTag, false, true);
                    break;
                case Rejected:
                    channel.Received(1).BasicNack(expectedDeliveryTag, false, false);
                    break;
            };
        }

        public static IEnumerable<object[]> Dispositions => new List<object[]>
        {
            new object[]{  new Accepted() },
            new object[]{  new Released() },
            new object[]{  new Modified() },
            new object[]{  new Rejected() }
        };
    }
}