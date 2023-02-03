using ServiceBusEmulator.Abstractions.Azure;
using ServiceBusEmulator.RabbitMq.Commands;

namespace ServiceBusEmulator.RabbitMq.Tests.Commands
{
    public class SetSessionStateCommandTest : Base<SetSessionStateCommand>
    {
        [Fact]
        public void ThatResponds()
        {
            (var response, var status) = Sut.Handle(null, null, null);

            Assert.Equal(AmqpResponseStatusCode.OK, status);
        }
    }
}
