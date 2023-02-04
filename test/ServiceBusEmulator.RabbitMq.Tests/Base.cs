using Amqp.Listener;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
using System.Reflection;

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
            }).Customize(new InternalConstructorCustomization());
        }
    }

    public abstract class Base<TSut> : Base
    {
        protected TSut Sut => Fixture.Freeze<TSut>(); 
    }

    public class InternalConstructorCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var internalConstructorQuery = new MethodInvoker(new InternalConstructorQuery());

            fixture.Customize<MessageContext>(c => c.FromFactory(internalConstructorQuery));
            fixture.Customize<DispositionContext>(c => c.FromFactory(internalConstructorQuery));
            fixture.Customize<FlowContext>(c => c.FromFactory(internalConstructorQuery));
        }

        private class InternalConstructorQuery : IMethodQuery
        {
            public IEnumerable<IMethod> SelectMethods(Type type)
            {
                if (type == null) { throw new ArgumentNullException(nameof(type)); }

                return from ci in type.GetTypeInfo()
                        .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                       select new ConstructorMethod(ci) as IMethod;
            }
        }
    }
}
