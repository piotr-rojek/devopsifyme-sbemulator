using ServiceBusEmulator.RabbitMq.Links;
using System.Runtime.Serialization;
using Amqp.Listener;

namespace ServiceBusEmulator.RabbitMq.Tests.Links
{
    public class RabbitMqLinkRegisterTest : Base<RabbitMqLinkRegister>
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("someAddress", true)]
        public void ThatItStoresAndFinds(string address, bool isValid)
        {
            var expectedLink = (ListenerLink)FormatterServices.GetUninitializedObject(typeof(ListenerLink));

            Sut.RegisterLink(address, expectedLink);
            var actualLink = Sut.FindByAddress(address);

            if(isValid)
            {
                Assert.Equal(expectedLink, actualLink);
            }
            else
            {
                Assert.Null(actualLink);
            }            
        }
    }
}