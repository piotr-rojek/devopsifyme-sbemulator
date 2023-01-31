using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using Microsoft.Extensions.Logging;
using Xim.Simulators.ServiceBus.InMemory.Delivering;
using Xim.Simulators.ServiceBus.InMemory.Endpoints;
using Xim.Simulators.ServiceBus.InMemory.Entities;

namespace Xim.Simulators.ServiceBus.InMemory
{
    internal class InMemoryLinkProcessor : ILinkProcessor
    {
        private readonly ISecurityContext _securityContext;
        private readonly IEntityLookup _entityLookup;
        private readonly ILogger _logger;

        public InMemoryLinkProcessor(ISecurityContext securityContext, IEntityLookup entityLookup, ILogger<InMemoryLinkProcessor> logger)
        {
            _securityContext = securityContext;
            _entityLookup = entityLookup;
            _logger = logger;
        }

        public void Process(AttachContext attachContext)
        {
            if (string.IsNullOrEmpty(attachContext.Attach.LinkName))
            {
                attachContext.Complete(new Error(ErrorCode.InvalidField) { Description = "Empty link name not allowed." });
                _logger.LogError($"Could not attach empty link to {GetType().Name}.");
                return;
            }

            if (!_securityContext.IsAuthorized(attachContext.Link.Session.Connection))
            {
                attachContext.Complete(new Error(ErrorCode.UnauthorizedAccess) { Description = "Not authorized." });
                _logger.LogError($"Could not attach unathorized link to {GetType().Name}.");
                return;
            }

            if (attachContext.Link.Role)
                AttachIncomingLink(attachContext, (Target)attachContext.Attach.Target);
            else
                AttachOutgoingLink(attachContext, (Source)attachContext.Attach.Source);
        }

        private void AttachIncomingLink(AttachContext attachContext, Target target)
        {
            IEntity entity = _entityLookup.Find(target.Address);
            if (entity == null)
            {
                attachContext.Complete(new Error(ErrorCode.NotFound) { Description = "Entity not found." });
                _logger.LogError($"Could not attach incoming link to non-existing entity '{target.Address}'.");
                return;
            }

            var incomingLinkEndpoint = new IncomingLinkEndpoint(entity);
            attachContext.Complete(incomingLinkEndpoint, 300);
            _logger.LogDebug($"Attached incoming link to entity '{target.Address}'.");
        }

        private void AttachOutgoingLink(AttachContext attachContext, Source source)
        {
            IEntity entity = _entityLookup.Find(source.Address);
            if (entity == null)
            {
                attachContext.Complete(new Error(ErrorCode.NotFound) { Description = "Entity not found." });
                _logger.LogError($"Could not attach outgoing link to non-existing entity '{source.Address}'.");
                return;
            }

            DeliveryQueue queue = entity.DeliveryQueue;
            if (queue == null)
            {
                attachContext.Complete(new Error(ErrorCode.NotFound) { Description = "Queue not found." });
                _logger.LogError($"Could not attach outgoing link to non-existing queue '{source.Address}'.");
                return;
            }

            var outgoingLinkEndpoint = new OutgoingLinkEndpoint(queue);
            attachContext.Complete(outgoingLinkEndpoint, 0);
            _logger.LogDebug($"Attached outgoing link to queue '{source.Address}'.");
        }
    }
}
