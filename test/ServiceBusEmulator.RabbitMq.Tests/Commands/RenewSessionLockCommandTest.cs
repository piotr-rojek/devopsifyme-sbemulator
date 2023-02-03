using Amqp;
using Amqp.Framing;
using Amqp.Types;
using AutoFixture;
using ServiceBusEmulator.Abstractions.Azure;
using ServiceBusEmulator.RabbitMq.Commands;

namespace ServiceBusEmulator.RabbitMq.Tests.Commands
{
    public class RenewSessionLockCommandTest : Base<RenewSessionLockCommand>
    {
        [Fact]
        public void ThatResponds()
        {
            var request = new Dictionary<string, object>
            {
                { ManagementConstants.Properties.LockTokens, Fixture.Create<Guid[]>() }
            };

            (var response, var status) = Sut.Handle(new Message(request.ToMap()), null, null);

            Assert.Equal(AmqpResponseStatusCode.OK, status);
            Assert.IsType<AmqpValue>(response.BodySection);

            var responseValues = ((AmqpValue)response.BodySection).Value as Map;
            Assert.NotNull(responseValues);

            var responseExpiration = (DateTime)responseValues[ManagementConstants.Properties.Expiration];
            Assert.True(responseExpiration > DateTime.UtcNow);
        }
    }
}
