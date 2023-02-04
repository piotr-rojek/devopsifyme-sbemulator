using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using RabbitMQ.Client;
using ServiceBusEmulator.Abstractions.Azure;
using ServiceBusEmulator.RabbitMq.Commands;
using ServiceBusEmulator.RabbitMq.Links;

namespace ServiceBusEmulator.RabbitMq.Endpoints
{
    public class RabbitMqManagementSenderEndpoint : LinkEndpointWithTargetContext
    {
        private readonly IRabbitMqManagementCommandFactory _commandFactory;
        private readonly IRabbitMqLinkRegister _receiverLinkFinder;

        public RabbitMqManagementSenderEndpoint(IRabbitMqManagementCommandFactory commandFactory, IRabbitMqLinkRegister receiverLinkFinder)
        {
            _commandFactory = commandFactory;
            _receiverLinkFinder = receiverLinkFinder;
        }

        protected IModel Channel { get; private set; } = null!;
        protected Target Target { get; private set; } = null!;

        public override void SetContext(IModel channel, Target target)
        {
            Channel = channel;
            Target = target;
        }

        public override void OnMessage(MessageContext messageContext)
        {
            try
            {
                Message response = GetResponse(messageContext.Message);

                ListenerLink? replyLink = _receiverLinkFinder.FindByAddress(messageContext.Message.Properties.ReplyTo)
                    ?? throw new Exception("Cannot find receiver's link");

                replyLink.SendMessage(response);
                messageContext.Complete();
            }
            catch
            {
                messageContext.Complete(new Error(ErrorCode.PreconditionFailed));
            }
        }

        public Message GetResponse(Message request)
        {
            string? operation = request.ApplicationProperties?.Map[ManagementConstants.Request.Operation] as string;

            IManagementCommand handler = _commandFactory.GetCommandHandler(operation);
            (Message? response, AmqpResponseStatusCode status) = handler.Handle(request, Channel, Target.Address);

            response.ApplicationProperties ??= new ApplicationProperties();
            response.ApplicationProperties[ManagementConstants.Response.StatusCode] = (int)status;
            response.ApplicationProperties[ManagementConstants.Response.StatusDescription] = status.ToString();

            response.Properties ??= new Properties();
            response.Properties.CorrelationId = request.Properties.MessageId;

            return response;
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
