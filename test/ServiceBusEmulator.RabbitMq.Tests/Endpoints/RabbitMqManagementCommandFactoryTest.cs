using AutoFixture;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using AutoFixture.Kernel;
using ServiceBusEmulator.RabbitMq.Endpoints;
using ServiceBusEmulator.Abstractions.Azure;
using ServiceBusEmulator.RabbitMq.Commands;

namespace ServiceBusEmulator.RabbitMq.Tests.Links
{
    public class RabbitMqManagementCommandFactoryTest : Base<RabbitMqManagementCommandFactory>
    {
        public RabbitMqManagementCommandFactoryTest()
        {
            Fixture.Freeze<IServiceProvider>().GetRequiredService<object>()
                .ReturnsForAnyArgs(ci => Fixture.Create(ci.ArgAt<Type>(0), new SpecimenContext(Fixture)));
        }

        [Theory]
        [InlineData(ManagementConstants.Operations.RenewLockOperation, nameof(RenewLockCommand))]
        [InlineData(ManagementConstants.Operations.PeekMessageOperation, nameof(PeekMessageCommand))]
        [InlineData(ManagementConstants.Operations.RenewSessionLockOperation, nameof(RenewSessionLockCommand))]
        [InlineData(ManagementConstants.Operations.SetSessionStateOperation, nameof(SetSessionStateCommand))]
        [InlineData(ManagementConstants.Operations.GetSessionStateOperation, nameof(GetSessionStateCommand))]
        public void ThatCommandIsReturned(string operation, string expectedTypeName)
        {
            var command = Sut.GetCommandHandler(operation);

            Assert.Equal(command.GetType().Name, expectedTypeName);
        }

        [Fact]
        public void ThatUnsupportedOperationIsHandled()
        {
            Assert.Throws<NotImplementedException>(() => Sut.GetCommandHandler("dummy"));  
        }
    }
}