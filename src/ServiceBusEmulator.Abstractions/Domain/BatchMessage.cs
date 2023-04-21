using Amqp;
using Amqp.Framing;
using Amqp.Types;

namespace ServiceBusEmulator.Abstractions.Domain
{
    /// <remarks>
    /// This is required as standard Message implementation does not handle multiple Data elements :(
    /// </remarks>
    public class BatchMessage : Message
    {
        private const ulong HeaderCode = 112uL;
        private const ulong DeliveryAnnotationsCode = 113uL;
        private const ulong MessageAnnotationsCode = 114uL;
        private const ulong PropertiesCode = 115uL;
        private const ulong ApplicationPropertiesCode = 116uL;
        private const ulong AmqpValueCode = 119uL;
        private const ulong DataCode = 117uL;
        private const ulong AmqpSequenceCode = 118uL;
        private const ulong FooterCode = 120uL;

        public List<Message> InnerMessages { get; set; } = new List<Message>();

        public static BatchMessage DecodeWithBatchSupport(ByteBuffer buffer)
        {
            BatchMessage message = new BatchMessage();
            while (buffer.Length > 0)
            {
                RestrictedDescribed restrictedDescribed = (RestrictedDescribed)Encoder.ReadDescribed(buffer, Encoder.ReadFormatCode(buffer));
                if (restrictedDescribed.Descriptor.Code == HeaderCode)
                {
                    message.Header = (Header)restrictedDescribed;
                    continue;
                }

                if (restrictedDescribed.Descriptor.Code == DeliveryAnnotationsCode)
                {
                    message.DeliveryAnnotations = (DeliveryAnnotations)restrictedDescribed;
                    continue;
                }

                if (restrictedDescribed.Descriptor.Code == MessageAnnotationsCode)
                {
                    message.MessageAnnotations = (MessageAnnotations)restrictedDescribed;
                    continue;
                }

                if (restrictedDescribed.Descriptor.Code == PropertiesCode)
                {
                    message.Properties = (Properties)restrictedDescribed;
                    continue;
                }

                if (restrictedDescribed.Descriptor.Code == ApplicationPropertiesCode)
                {
                    message.ApplicationProperties = (ApplicationProperties)restrictedDescribed;
                    continue;
                }

                if (restrictedDescribed.Descriptor.Code == DataCode && restrictedDescribed is Data dataBody)
                {
                    try
                    {
                        message.InnerMessages.Add(Decode(dataBody.Buffer));
                    }
                    catch (AmqpException)
                    {
                        // apparently not a batch :O
                    }
                }

                if (restrictedDescribed.Descriptor.Code == AmqpValueCode || restrictedDescribed.Descriptor.Code == DataCode || restrictedDescribed.Descriptor.Code == AmqpSequenceCode)
                {
                    message.BodySection = restrictedDescribed;

                    continue;
                }

                if (restrictedDescribed.Descriptor.Code == FooterCode)
                {
                    message.Footer = (Footer)restrictedDescribed;
                    continue;
                }

                throw new AmqpException("amqp:connection:framing-error", Fx.Format(SRAmqp.AmqpUnknownDescriptor, restrictedDescribed.Descriptor));
            }

            return message;
        }
    }
}
