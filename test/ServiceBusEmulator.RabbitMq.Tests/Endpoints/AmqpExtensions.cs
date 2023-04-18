using System.Runtime.Serialization;
using Amqp.Listener;
using AutoFixture;
using Amqp;
using Amqp.Handler;
using Amqp.Framing;
using System.Reflection;
using Amqp.Types;
using System.Text;

namespace ServiceBusEmulator.RabbitMq.Tests.Links
{
    public static class AmqpExtensions
    {
        public static IFixture RegisterAmqpDummyTypes(this IFixture fixture)
        {
            fixture.Register<ListenerLink>(() => (ListenerLink)FormatterServices.GetUninitializedObject(typeof(ListenerLink)));


            return fixture;
        }

        public static IFixture CustomizeAmqpMessage(this IFixture fixture, RestrictedDescribed body = null, DeliveryState deliveryState = null)
        {
            fixture.Customize<Message>(entity => entity
                .With(x => x.BodySection, body ?? new Data()
                {
                    Binary = Encoding.UTF8.GetBytes(fixture.Create<string>())
                })
                .Do(x =>
                {
                    var deliverySetter = typeof(Message).GetTypeInfo().GetProperty("Delivery", BindingFlags.NonPublic | BindingFlags.Instance)!;

                    var delivery = (IDelivery)FormatterServices.GetUninitializedObject(deliverySetter.PropertyType);
                    delivery.State = deliveryState ?? new Accepted();
                    delivery.Tag = fixture.CreateMany<byte>(16).ToArray();

                    deliverySetter.PropertyType.GetField("Buffer").SetValue(delivery, new ByteBuffer(1, true));

                    deliverySetter.SetValue(x, delivery);
                })
            );

            return fixture;
        }
    }
}