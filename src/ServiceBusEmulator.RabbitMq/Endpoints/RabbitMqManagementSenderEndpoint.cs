using Amqp.Framing;
using Amqp.Listener;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using ServiceBusEmulator.Abstractions.Azure;
using ServiceBusEmulator.RabbitMq.Commands;

namespace ServiceBusEmulator.RabbitMq.Endpoints
{
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

        protected IModel Channel { get; private set; } = null!;
        protected Target Target { get; private set; } = null!;

        public void SetContext(IModel channel, Target target)
        {
            Channel = channel;
            Target = target;
        }

        public override void OnMessage(MessageContext messageContext)
        {
            Amqp.Message request = messageContext.Message;
            string? operation = request.ApplicationProperties?.Map[ManagementConstants.Request.Operation] as string;
            IManagementCommand handler = GetCommandHandler(operation);

            (Amqp.Message? response, AmqpResponseStatusCode status) = handler.Handle(request, Channel, Target.Address);

            using (response)
            {
                response.ApplicationProperties ??= new ApplicationProperties();
                response.ApplicationProperties[ManagementConstants.Response.StatusCode] = (int)status;
                response.ApplicationProperties[ManagementConstants.Response.StatusDescription] = status.ToString();

                response.Properties ??= new Properties();
                response.Properties.CorrelationId = request.Properties.MessageId;

                ListenerLink? replyLink = _receiverLinkFinder.FindByAddress(messageContext.Message.Properties.ReplyTo);
                if(replyLink == null)
                {
                    messageContext.Complete(new Error(Amqp.ErrorCode.PreconditionFailed));
                    return;
                }

                replyLink.SendMessage(response);
                messageContext.Complete();
            }
        }

        private IManagementCommand GetCommandHandler(string? operation)
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
