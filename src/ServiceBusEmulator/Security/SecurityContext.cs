using System;
using System.Reflection;
using System.Security.Principal;
using Amqp;
using Amqp.Framing;
using Amqp.Listener;

namespace Xim.Simulators.ServiceBus.Security
{
    internal sealed class SecurityContext : ISecurityContext
    {
        private static readonly IPrincipal XimPrincipal = new GenericPrincipal(new GenericIdentity("Xim"), null);

        internal static SecurityContext Default { get; } = new SecurityContext();

        public void Authorize(Connection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (connection is ListenerConnection listenerConnection)
            {
                SetPrincipal(listenerConnection, XimPrincipal);
                listenerConnection.AddClosedCallback(ConnectionClosed);
            }
        }

        public bool IsAuthorized(Connection connection)
            => (connection is ListenerConnection listenerConnection)
                && listenerConnection.Principal == XimPrincipal;

        private void ConnectionClosed(IAmqpObject sender, Error error)
        {
            SetPrincipal((ListenerConnection)sender, null);
            sender.Closed -= ConnectionClosed;
        }

        private static void SetPrincipal(ListenerConnection connection, IPrincipal principal)
            => connection
                .GetType()
                .GetProperty(nameof(ListenerConnection.Principal), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .SetValue(connection, principal);
    }
}
