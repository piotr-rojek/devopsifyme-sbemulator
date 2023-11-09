using Amqp;
using Amqp.Framing;
using Amqp.Types;
using RabbitMQ.Client;
using System.Globalization;

namespace ServiceBusEmulator.RabbitMq
{
    public class RabbitMqMapper : IRabbitMqMapper
    {
        public void MapFromRabbit(Message message, ReadOnlyMemory<byte> body, IBasicProperties prop)
        {
            message.BodySection = prop.Type switch
            {
                "amqp:data:binary" => new Data(),
                "amqp:amqp-value:*" => new AmqpValue(),
                "amqp:amqp-sequence:list" => new AmqpSequence(),
                _ => null,
            };
            if (message.BodySection == null)
            {
                message.BodySection = new Data
                {
                    Buffer = new ByteBuffer(body.ToArray(), 0, body.Length, body.Length)
                };
            }
            else
            {
                message.BodySection.Decode(new ByteBuffer(body.ToArray(), 0, body.Length, body.Length));
            }

            message.Header = new Header
            {
                DeliveryCount = prop.GetHeader<uint>("x-delivery-count"),
                Ttl = string.IsNullOrEmpty(prop.Expiration) ? 0 : Convert.ToUInt32(prop.Expiration),
                Durable = prop.Persistent,
                Priority = prop.Priority
            };

            message.ApplicationProperties = new ApplicationProperties();
            foreach ((string key, object? value) in prop.GetHeadersStartingWith<object>("x-sb-app-"))
            {
                message.ApplicationProperties[key] = value;
            }

            message.MessageAnnotations = new MessageAnnotations();
            foreach ((string key, object? value) in prop.GetHeadersStartingWith<object>("x-sb-annotation-"))
            {
                message.MessageAnnotations[new Symbol(key)] = value;
            }
            message.MessageAnnotations[new Symbol("x-opt-locked-until")] = DateTime.UtcNow.AddDays(1);

            message.Properties = new Properties
            {
                AbsoluteExpiryTime = prop.GetHeader<DateTime>("x-sb-absolute-expiry-time"),
                ContentEncoding = prop.ContentEncoding,
                ContentType = prop.ContentType,
                CorrelationId = prop.CorrelationId,
                CreationTime = new DateTime(1970, 1, 1).AddSeconds(prop.Timestamp.UnixTime),
                GroupId = prop.GetHeader<string>("x-sb-group-id"),
                GroupSequence = prop.GetHeader<uint>("x-sb-group-seq"),
                ReplyToGroupId = prop.GetHeader<string>("x-sb-replyto-group-id"),
                MessageId = prop.MessageId,
                ReplyTo = prop.ReplyTo,
                Subject = prop.GetHeader<string>("x-sb-subject")
            };
        }

        public byte[] MapToRabbit(IBasicProperties prop, Message rMessage)
        {
            prop.Headers ??= new Dictionary<string, object>();
            byte[] data = new byte[0];

            if (rMessage.BodySection != null)
            {
                if (rMessage.BodySection is Data dataBody)
                {
                    //don't encode binary, we want to see actual value in rabbitmq ui
                    data = dataBody.Binary;
                }
                else
                {
                    prop.Type = rMessage.BodySection.Descriptor.Name;
                    ByteBuffer buffer = new(1024, true);
                    rMessage.BodySection.Encode(buffer);
                    data = buffer.Buffer[0..buffer.Length].ToArray();
                }
            }

            if (rMessage.Header != null)
            {
                prop.Persistent = rMessage.Header.Durable;
                prop.Priority = rMessage.Header.Priority;
                prop.Expiration = rMessage.Header.Ttl.ToString(CultureInfo.InvariantCulture);
                prop.Headers["x-delivery-count"] = rMessage.Header.DeliveryCount;
            }

            Properties rProperties = rMessage.Properties;
            if (rProperties != null)
            {
                DateTime creationTime = rProperties.CreationTime == DateTime.MinValue ? DateTime.UtcNow : rProperties.CreationTime;

                prop.ReplyTo = rProperties.ReplyTo;
                prop.MessageId = rProperties.MessageId ?? Guid.NewGuid().ToString();
                prop.CorrelationId = rProperties.CorrelationId;
                prop.ContentType = rProperties.ContentType;
                prop.ContentEncoding = rProperties.ContentEncoding;
                prop.Timestamp = new AmqpTimestamp((int)creationTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);


                prop.Headers["x-sb-subject"] = rProperties.Subject;
                prop.Headers["x-sb-group-seq"] = rProperties.GroupSequence == 0 ? null : rProperties.GroupSequence;
                prop.Headers["x-sb-group-id"] = rProperties.GroupId;
                prop.Headers["x-sb-replyto-group-id"] = rProperties.ReplyToGroupId;
                prop.Headers["x-sb-absolute-expiry-time"] = rProperties.AbsoluteExpiryTime.ToString("o", CultureInfo.InvariantCulture);
            }

            if (rMessage.ApplicationProperties != null)
            {
                foreach (KeyValuePair<object, object?> p in rMessage.ApplicationProperties.Map)
                {
                    prop.Headers[$"x-sb-app-{p.Key}"] = p.Value?.ToString();
                }
            }

            if (rMessage.MessageAnnotations != null)
            {
                foreach (KeyValuePair<object, object?> p in rMessage.MessageAnnotations.Map)
                {
                    prop.Headers[$"x-sb-annotation-{p.Key}"] = p.Value?.ToString();
                }
            }

            //remove empty values
            foreach (KeyValuePair<string, object> it in prop.Headers.Where(it => null == it.Value))
            {
                _ = prop.Headers.Remove(it.Key);
            }

            return data;
        }
    }
}
