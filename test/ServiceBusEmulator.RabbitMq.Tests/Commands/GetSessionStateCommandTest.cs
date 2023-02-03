using Amqp.Types;
using ServiceBusEmulator.Abstractions.Azure;
using ServiceBusEmulator.RabbitMq.Commands;

namespace ServiceBusEmulator.RabbitMq.Tests.Commands
{
    public class GetSessionStateCommandTest : Base<GetSessionStateCommand>
    {
        [Fact]
        public void ThatResponds()
        {
            (var response, var status) = Sut.Handle(null, null, null);

            Assert.Equal(AmqpResponseStatusCode.OK, status);
            Assert.Equal(new byte[0], ((Map)response.Body)[ManagementConstants.Properties.SessionState]);
        }
    }
}
