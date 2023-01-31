using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xim.Simulators.ServiceBus.Azure
{
    public static class ManagementConstants
    {
        public const string Microsoft = "com.microsoft";

        public static class Request
        {
            public const string Operation = "operation";
            public const string AssociatedLinkName = "associated-link-name";
        }

        public static class Response
        {
            public const string StatusCode = "statusCode";
            public const string StatusDescription = "statusDescription";
            public const string ErrorCondition = "errorCondition";
        }

        public static class Operations
        {
            public const string RenewLockOperation = Microsoft + ":renew-lock";
            public const string ReceiveBySequenceNumberOperation = Microsoft + ":receive-by-sequence-number";
            public const string UpdateDispositionOperation = Microsoft + ":update-disposition";
            public const string RenewSessionLockOperation = Microsoft + ":renew-session-lock";
            public const string SetSessionStateOperation = Microsoft + ":set-session-state";
            public const string GetSessionStateOperation = Microsoft + ":get-session-state";
            public const string PeekMessageOperation = Microsoft + ":peek-message";
            public const string AddRuleOperation = Microsoft + ":add-rule";
            public const string RemoveRuleOperation = Microsoft + ":remove-rule";
            public const string EnumerateRulesOperation = Microsoft + ":enumerate-rules";
            public const string ScheduleMessageOperation = Microsoft + ":schedule-message";
            public const string CancelScheduledMessageOperation = Microsoft + ":cancel-scheduled-message";
        }

        public static class Properties
        {
            public static readonly string ServerTimeout = Microsoft + ":server-timeout";
            public static readonly string TrackingId = Microsoft + ":tracking-id";

            public static readonly string SessionState = "session-state";
            public static readonly string LockToken = "lock-token";
            public static readonly string LockTokens = "lock-tokens";
            public static readonly string SequenceNumbers = "sequence-numbers";
            public static readonly string Expirations = "expirations";
            public static readonly string Expiration = "expiration";
            public static readonly string SessionId = "session-id";
            public static readonly string MessageId = "message-id";
            public static readonly string PartitionKey = "partition-key";
            public static readonly string ViaPartitionKey = "via-partition-key";

            public static readonly string ReceiverSettleMode = "receiver-settle-mode";
            public static readonly string Message = "message";
            public static readonly string Messages = "messages";
            public static readonly string DispositionStatus = "disposition-status";
            public static readonly string PropertiesToModify = "properties-to-modify";
            public static readonly string DeadLetterReason = "deadletter-reason";
            public static readonly string DeadLetterDescription = "deadletter-description";

            public static readonly string FromSequenceNumber = "from-sequence-number";
            public static readonly string MessageCount = "message-count";

            public static readonly string Skip = "skip";
            public static readonly string Top = "top";
            public static readonly string Rules = "rules";
            public static readonly string RuleName = "rule-name";
            public static readonly string RuleDescription = "rule-description";
            public static readonly string RuleCreatedAt = "rule-created-at";
            public static readonly string SqlFilter = "sql-filter";
            public static readonly string SqlRuleAction = "sql-rule-action";
            public static readonly string CorrelationFilter = "correlation-filter";
            public static readonly string Expression = "expression";
            public static readonly string CorrelationId = "correlation-id";
            public static readonly string To = "to";
            public static readonly string ReplyTo = "reply-to";
            public static readonly string Label = "label";
            public static readonly string ReplyToSessionId = "reply-to-session-id";
            public static readonly string ContentType = "content-type";
            public static readonly string CorrelationFilterProperties = "properties";
        }
    }
}
