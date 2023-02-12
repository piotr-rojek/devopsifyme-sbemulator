using Microsoft.Extensions.Diagnostics.HealthChecks;
using ServiceBusEmulator.RabbitMq.Links;

namespace ServiceBusEmulator.RabbitMq
{
    public class RabbitMqHealthCheck : IHealthCheck
    {
        private readonly IRabbitMqChannelFactory _channelFactory;

        public RabbitMqHealthCheck(IRabbitMqChannelFactory channelFactory)
        {
            _channelFactory = channelFactory;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var model = _channelFactory.CreateChannel();
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception ex)
            {
                return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, exception: ex));
            }
        }
    }
}
