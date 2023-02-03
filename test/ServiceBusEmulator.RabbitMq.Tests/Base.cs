using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace ServiceBusEmulator.RabbitMq.Tests
{
    public abstract class Base
    {
        protected IFixture Fixture { get; }

        public Base()
        {
            Fixture = new Fixture().Customize(new AutoNSubstituteCustomization
            {
                ConfigureMembers= true
            });
        }
    }

    public abstract class Base<TSut> : Base
    {
        protected TSut Sut => Fixture.Freeze<TSut>(); 
    }
}
