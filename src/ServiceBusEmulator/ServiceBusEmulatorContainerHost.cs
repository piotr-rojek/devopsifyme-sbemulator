using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using ServiceBusEmulator.Abstractions.Domain;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace ServiceBusEmulator
{
    internal class ServiceBusEmulatorContainerHost : ContainerHost, IContainer
    {
        public ServiceBusEmulatorContainerHost(IList<Address> addressList, X509Certificate2 certificate) : base(addressList, certificate)
        {
            AddressResolver += (c, attach) =>
            {
                // required for node.js SDK $cbs authentication
                ((Target)attach.Target).Address ??= attach.LinkName;
                return null;
            };
        }

        Message IContainer.CreateMessage(ByteBuffer buffer)
        {
            // required for batched messages support
            // ideally we would have access to Transfer here to know if this is a batched message or not..
            return BatchMessage.DecodeWithBatchSupport(buffer);
        }
    }
}
