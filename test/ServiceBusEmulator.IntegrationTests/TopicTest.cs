using AutoFixture;
using Azure.Messaging.ServiceBus;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace ServiceBusEmulator.IntegrationTests
{
    [Collection(Consts.TopicCollection)]
    public class TopicTest : Base
    {
        [Fact]
        public async Task ThatMessageIsReceived()
        {
            string messageBody = Fixture.Create<string>();

            var sender = Client.CreateSender(Consts.TestTopicName);
            var receiver1 = Client.CreateReceiver(Consts.TestSubsciption1Name, new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
            });
            var receiver2 = Client.CreateReceiver(Consts.TestSubsciption2Name, new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
            });

            await sender.SendMessageAsync(new ServiceBusMessage(messageBody));
            var receivedMessage1 = await receiver1.ReceiveMessageAsync();
            var nextMessage1 = await receiver1.PeekMessageAsync();
            var receivedMessage2 = await receiver2.ReceiveMessageAsync();
            var nextMessage2 = await receiver2.PeekMessageAsync();

            Assert.Multiple(
                () => Assert.Equal(messageBody, receivedMessage1.Body.ToString()),
                () => Assert.Equal(messageBody, receivedMessage2.Body.ToString()),
                () => Assert.Null(nextMessage1),
                () => Assert.Null(nextMessage2)
            );
        }
    }
}