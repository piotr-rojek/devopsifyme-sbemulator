using Amqp;
using Amqp.Sasl;
using Amqp.Types;
using System.Text;

namespace ServiceBusEmulator.Azure
{
    internal class AzureSaslProfile : SaslProfile
    {
        private const string CbsSaslMechanismName = "MSSBCBS";

        private static readonly Descriptor SaslInit = new(0x0000000000000041, "amqp:sasl-init:list");
        private static readonly Descriptor SaslMechanisms = new(0x0000000000000040, "amqp:sasl-mechanisms:list");

        public AzureSaslProfile() : base(CbsSaslMechanismName) { }

        protected override ITransport UpgradeTransport(ITransport transport)
        {
            return transport;
        }

        protected override DescribedList GetStartCommand(string hostname)
        {
            return new SaslInit
            {
                Mechanism = Mechanism,
                InitialResponse = Encoding.UTF8.GetBytes(CbsSaslMechanismName)
            };
        }

        protected override DescribedList OnCommand(DescribedList command)
        {
            if (command.Descriptor.Code == SaslInit.Code)
            {
                return new SaslOutcome { Code = SaslCode.Ok };
            }
            else if (command.Descriptor.Code == SaslMechanisms.Code)
            {
                return null;
            }

            throw new AmqpException(ErrorCode.NotAllowed, command.ToString());
        }
    }
}
