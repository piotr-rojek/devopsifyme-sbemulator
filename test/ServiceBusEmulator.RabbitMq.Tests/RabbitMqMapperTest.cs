using Amqp.Framing;
using Amqp.Types;
using AutoFixture;
using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq.Tests
{
    public class RabbitMqMapperTest : Base<RabbitMqMapper>
    {
        /// <summary>
        /// Assertions that are commented out are currently not supported 
        /// </summary>
        [Fact]
        public void ThatMappingIsSimetrical()
        {
            var expectedMessage = Fixture.Build<Amqp.Message>()
                .With(x => x.BodySection, Fixture.Build<Data>().With(x => x.Buffer, new Amqp.ByteBuffer(Fixture.CreateMany<byte>(255).ToArray(), 0, 255, 255)).Create())
                .With(x => x.ApplicationProperties, Fixture.CreateDescribedMap<ApplicationProperties, string, string>())
                .With(x => x.DeliveryAnnotations, Fixture.CreateDescribedMap<DeliveryAnnotations, Symbol, string>())
                .With(x => x.Footer, Fixture.CreateDescribedMap<Footer, Symbol, string>())
                .With(x => x.MessageAnnotations, Fixture.CreateDescribedMap<MessageAnnotations, Symbol, string>())
                .Create();

            var rabbitProperties = Fixture.Create<IBasicProperties>();
            var rabbitPayload = Sut.MapToRabbit(rabbitProperties, expectedMessage);

            var amqpMessage = new Amqp.Message();
            Sut.MapFromRabbit(amqpMessage, rabbitPayload, rabbitProperties);

            Assert.Multiple(
                () => Assert.Equal(expectedMessage.Body, amqpMessage.Body),

                () => Assert.Equal(expectedMessage.Header.Durable, amqpMessage.Header.Durable),
                () => Assert.Equal(expectedMessage.Header.Priority, amqpMessage.Header.Priority),
                () => Assert.Equal(expectedMessage.Header.Ttl, amqpMessage.Header.Ttl),
                //() => Assert.Equal(expectedMessage.Header.FirstAcquirer, amqpMessage.Header.FirstAcquirer),
                () => Assert.Equal(expectedMessage.Header.DeliveryCount, amqpMessage.Header.DeliveryCount),

                () => Assert.Equal(expectedMessage.Properties.MessageId, amqpMessage.Properties.MessageId),
                //() => Assert.Equal(expectedMessage.Properties.UserId, amqpMessage.Properties.UserId),
                //() => Assert.Equal(expectedMessage.Properties.To, amqpMessage.Properties.To),
                () => Assert.Equal(expectedMessage.Properties.Subject, amqpMessage.Properties.Subject),
                () => Assert.Equal(expectedMessage.Properties.ReplyTo, amqpMessage.Properties.ReplyTo),
                () => Assert.Equal(expectedMessage.Properties.CorrelationId, amqpMessage.Properties.CorrelationId),
                () => Assert.Equal(expectedMessage.Properties.ContentType, amqpMessage.Properties.ContentType),
                () => Assert.Equal(expectedMessage.Properties.ContentEncoding, amqpMessage.Properties.ContentEncoding),
                () => Assert.Equal(expectedMessage.Properties.AbsoluteExpiryTime, amqpMessage.Properties.AbsoluteExpiryTime),
                () => Assert.Equal(expectedMessage.Properties.CreationTime, amqpMessage.Properties.CreationTime, TimeSpan.FromSeconds(1)),
                () => Assert.Equal(expectedMessage.Properties.GroupId, amqpMessage.Properties.GroupId),
                () => Assert.Equal(expectedMessage.Properties.GroupSequence, amqpMessage.Properties.GroupSequence),
                () => Assert.Equal(expectedMessage.Properties.ReplyToGroupId, amqpMessage.Properties.ReplyToGroupId),


                () => Assert.Equal(expectedMessage.ApplicationProperties.FromDescribedMap(), amqpMessage.ApplicationProperties.FromDescribedMap()),
                //() => Assert.Equal(expectedMessage.DeliveryAnnotations.FromDescribedMap(), amqpMessage.DeliveryAnnotations.FromDescribedMap()),
                //() => Assert.Equal(expectedMessage.Footer.FromDescribedMap(), amqpMessage.Footer.FromDescribedMap()),
                () => Assert.Equal(expectedMessage.MessageAnnotations.FromDescribedMap(), amqpMessage.MessageAnnotations.FromDescribedMap()),

                () => Assert.Equal((DateTime)amqpMessage.MessageAnnotations[new Symbol("x-opt-locked-until")], DateTime.UtcNow, TimeSpan.FromDays(2))
            );
        }

        
    }
}