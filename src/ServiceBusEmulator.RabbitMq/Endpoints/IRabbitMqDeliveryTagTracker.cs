namespace ServiceBusEmulator.RabbitMq.Endpoints
{
    public interface IRabbitMqDeliveryTagTracker
    {
        ulong? this[byte[] amqpDeliveryTag] { get; set; }
    }
}
