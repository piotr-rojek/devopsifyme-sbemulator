using Amqp.Framing;
using Amqp.Listener;
using System;
using RabbitMQ.Client;
using Xim.Simulators.ServiceBus.InMemory;
using Xim.Simulators.ServiceBus.Rabbit.Management;
using Xim.Simulators.ServiceBus.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace Xim.Simulators.ServiceBus.Rabbit.Endpoints
{
    public class RabbitMqManagementReceiverEndpoint : LinkEndpoint
    {
        public IModel Channel { get; private set; }

        public void SetContext(IModel channel)
        {
            Channel = channel;
        }

        public override void OnDisposition(DispositionContext dispositionContext)
        {
            dispositionContext.Complete();
        }

        public override void OnFlow(FlowContext flowContext)
        {
        }

        public override void OnLinkClosed(ListenerLink link, Error error)
        {
            Channel.Dispose();
        }
    }


    /// <summary>
    /// https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-amqp-request-response
    /// </summary>
    public class RabbitMqManagementSenderEndpoint : LinkEndpoint
    {

        private readonly IServiceProvider _services;
        private readonly IReceiverLinkFinder _receiverLinkFinder;

        public RabbitMqManagementSenderEndpoint(IServiceProvider services, IReceiverLinkFinder receiverLinkFinder)
        {
            _services = services;
            _receiverLinkFinder = receiverLinkFinder;
        }

        protected IModel Channel { get; private set; }
        protected Target Target { get; private set; }

        public void SetContext(IModel channel, Target target)
        {
            Channel = channel;
            Target = target;
        }

        public override void OnMessage(MessageContext messageContext)
        {
            var request = messageContext.Message;
            var operation = request.ApplicationProperties?.Map[ManagementConstants.Request.Operation] as string;
            var handler = GetCommandHandler(operation);

            (var response, var status) = handler.Handle(request, Channel, Target.Address);

            using (response)
            {
                response.ApplicationProperties ??= new ApplicationProperties();
                response.ApplicationProperties[ManagementConstants.Response.StatusCode] = (int)status;
                response.ApplicationProperties[ManagementConstants.Response.StatusDescription] = status.ToString();

                response.Properties ??= new Properties();
                response.Properties.CorrelationId = request.Properties.MessageId;

                var replyLink = _receiverLinkFinder.FindByAddress(messageContext.Message.Properties.ReplyTo);
                replyLink.SendMessage(response);

                messageContext.Complete();
            }
        }

        private IManagementCommand GetCommandHandler(string operation)
        {
            return operation switch
            {
                ManagementConstants.Operations.RenewLockOperation => _services.GetRequiredService<RenewLockCommand>(),
                ManagementConstants.Operations.PeekMessageOperation => _services.GetRequiredService<PeekMessageCommand>(),
                ManagementConstants.Operations.RenewSessionLockOperation => _services.GetRequiredService<RenewSessionLockCommand>(),
                ManagementConstants.Operations.SetSessionStateOperation => _services.GetRequiredService<SetSessionStateCommand>(),
                ManagementConstants.Operations.GetSessionStateOperation => _services.GetRequiredService<GetSessionStateCommand>(),
                _ => throw new NotImplementedException($"Operation {operation} not implemented")
            };

        }

        public override void OnFlow(FlowContext flowContext)
        {
            throw new NotImplementedException();
        }

        public override void OnDisposition(DispositionContext dispositionContext)
        {
            throw new NotImplementedException();
        }
    }
}
