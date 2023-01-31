using Amqp;
using Amqp.Listener;
using Amqp.Types;
using ServiceBusEmulator.Azure;

namespace ServiceBusEmulator
{
    internal static class AmqpExtensions
    {
        private static readonly Symbol XOptSequenceNumber = "x-opt-sequence-number";

        internal static Message Clone(this Message message)
        {
            return message == null
                        ? null
                        : Message.Decode(message.Encode());
        }

        internal static Message AddSequenceNumber(this Message message, long sequence)
        {
            if (message != null)
            {
                message.MessageAnnotations ??= new Amqp.Framing.MessageAnnotations();
                if (message.MessageAnnotations[XOptSequenceNumber] == null)
                {
                    message.MessageAnnotations[XOptSequenceNumber] = sequence;
                }
            }
            return message;
        }

        public static void EnableAzureSaslMechanism(this ConnectionListener.SaslSettings sasl)
        {
            var profile = new AzureSaslProfile();
            sasl.EnableMechanism(profile.Mechanism, profile);
        }
    }
}
