using Amqp.Framing;
using AutoFixture;
using RabbitMQ.Client;
using ServiceBusEmulator.RabbitMq.Links;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using AutoFixture.Kernel;
using ServiceBusEmulator.RabbitMq.Endpoints;

namespace ServiceBusEmulator.RabbitMq.Tests.Links
{
    public class RabbitMqLinkEndpointFactoryTest : Base<RabbitMqLinkEndpointFactory>
    {
        public RabbitMqLinkEndpointFactoryTest()
        {
            Fixture.Freeze<IServiceProvider>().GetRequiredService<object>()
                .ReturnsForAnyArgs(ci => Fixture.Create(ci.ArgAt<Type>(0), new SpecimenContext(Fixture)));
        }

        [Theory]
        [InlineData("queueName", false)]
        [InlineData("queueName/$management", true)]
        public void ThatSenderIsCreated(string address, bool isManagement)
        {
            var channel = Fixture.Create<IModel>();
            var target = Fixture.Build<Target>()
                .With(x => x.Address, address)
                .Create();

            var linkEndpoint = Sut.CreateSenderLinkEndpoint(channel, target);

            Assert.IsType(isManagement ? typeof(RabbitMqManagementSenderEndpoint) : typeof(RabbitMqSenderEndpoint), linkEndpoint);
        }

        [Theory]
        [InlineData("queueName", false)]
        [InlineData("queueName/$management", true)]
        public void ThatReceiverIsCreated(string address, bool isManagement)
        {
            var channel = Fixture.Create<IModel>();
            var rcvSettleMode = Fixture.Create<ReceiverSettleMode>();
            var source = Fixture.Build<Source>()
                .Without(x => x.Outcomes)
                .Without(x => x.DefaultOutcome)
                .With(x => x.Address, address)
                .Create();

            var linkEndpoint = Sut.CreateReceiverEndpoint(channel, source, rcvSettleMode);

            Assert.IsType(isManagement ? typeof(RabbitMqManagementReceiverEndpoint) : typeof(RabbitMqReceiverEndpoint), linkEndpoint);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ThatLinkIsCreated(bool isSender)
        {
            var channel = Fixture.Create<IModel>();
            var attach = Fixture.Build<Attach>()
                .With(x => x.Source, Fixture.Build<Source>().Without(x => x.Outcomes).Without(x => x.DefaultOutcome).Create())
                .With(x => x.Target, Fixture.Create<Target>())
                .Create();

            (var linkEndpoint, int initialCredit) = Sut.CreateEndpoint(channel, attach, isSender);

            Assert.Equal(initialCredit, isSender ? 300 : 0);
            Assert.IsAssignableFrom(isSender ? typeof(LinkEndpointWithTargetContext) : typeof(LinkEndpointWithSourceContext), linkEndpoint);
        }
    }
}