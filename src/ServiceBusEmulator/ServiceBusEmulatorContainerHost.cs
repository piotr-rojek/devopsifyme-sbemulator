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
    }
}
