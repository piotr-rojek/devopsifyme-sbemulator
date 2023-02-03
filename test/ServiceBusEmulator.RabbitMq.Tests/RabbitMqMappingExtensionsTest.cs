using AutoFixture;
using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq.Tests
{
    public class RabbitMqMappingExtensionsTest : Base
    {
        [Fact]
        public void ThatHeaderIsReturned()
        {
            var properties = Fixture.Create<IBasicProperties>();
            properties.Headers = new Dictionary<string, object>
            {
                { "item1", "value1" },
                { "item2", "value2" },
                { "item3", "value3" }
            };

            var result = RabbitMqMappingExtensions.GetHeader<string>(properties, "item2");

            Assert.Equal("value2", result);
        }

        [Fact]
        public void ThatHeadersAreFound()
        {
            var properties = Fixture.Create<IBasicProperties>();
            properties.Headers = new Dictionary<string, object>
            {
                { "item1", "value1" },
                { "item2", "value2" },
                { "item3", "value3" },
                { "other", "nothing" }
            };

            var result = RabbitMqMappingExtensions.GetHeadersStartingWith<string>(properties, "item");

            Assert.Collection(result,
                (it) => Assert.True(it.value?.StartsWith("value1")),
                (it) => Assert.True(it.value?.StartsWith("value2")),
                (it) => Assert.True(it.value?.StartsWith("value3"))
            );
        }
    }
}