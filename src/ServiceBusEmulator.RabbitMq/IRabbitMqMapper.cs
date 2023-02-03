using Amqp;
using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq
{
    public interface IRabbitMqMapper
    {
        void MapFromRabbit(Message message, ReadOnlyMemory<byte> body, IBasicProperties prop);
        byte[] MapToRabbit(IBasicProperties prop, Message rMessage);
    }
}
