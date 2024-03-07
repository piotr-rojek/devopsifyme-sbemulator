using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using Microsoft.Extensions.Logging;
using ServiceBusEmulator.Abstractions.Security;
using System;

namespace ServiceBusEmulator.Security
{
    public class CbsRequestProcessor : IRequestProcessor
    {
        private readonly ISecurityContext _messageContext;
        private readonly ILogger _logger;
        private readonly ITokenValidator _tokenValidator;

        public int Credit => 100;

        public CbsRequestProcessor(ISecurityContext messageContext, ILogger<CbsRequestProcessor> logger, ITokenValidator tokenValidator)
        {
            _messageContext = messageContext;
            _logger = logger;
            _tokenValidator = tokenValidator;
        }

        public void Process(RequestContext requestContext)
        {
            if (ValidateCbsRequest(requestContext))
            {
                _messageContext.Authorize(requestContext.Link.Session.Connection);
                using Message message = GetResponseMessage(200, requestContext);
                requestContext.Complete(message);
            }
            else
            {
                using (Message message = GetResponseMessage(401, requestContext))
                {
                    requestContext.Complete(message);
                }
                requestContext.ResponseLink.Close();
                requestContext.ResponseLink.AddClosedCallback((sender, _) => ((Link)sender).Session.Connection.CloseAsync());
            }
        }

        private bool ValidateCbsRequest(RequestContext requestContext)
        {
            string token = (string)requestContext.Message.Body;
            try
            {
                _tokenValidator.Validate(token);
                _logger.LogDebug($"Valid $cbs request; {token}.");
                return true;
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, $"Failed to validate $cbs request; {token}.");
                return false;
            }
        }

        private static Message GetResponseMessage(int responseCode, RequestContext requestContext)
        {
            string messageId = requestContext.Message.Properties.GetMessageId().ToString();
            requestContext.Message.Properties.SetMessageId(messageId);
            return new Message
            {
                Properties = new Properties
                {
                    CorrelationId = messageId,
                    MessageId = messageId
                },
                ApplicationProperties = new ApplicationProperties
                {
                    ["status-code"] = responseCode
                }
            };
        }
    }
}
