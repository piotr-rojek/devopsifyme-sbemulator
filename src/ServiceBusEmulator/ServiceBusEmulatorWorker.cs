using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xim.Simulators.ServiceBus;

namespace ServiceBusEmulator.Host
{
    public class ServiceBusEmulatorWorker : BackgroundService
    {
        private readonly ILogger<ServiceBusEmulatorWorker> _logger;
        private readonly ServiceBusEmulatorHost _emulator;

        public ServiceBusEmulatorWorker(ILogger<ServiceBusEmulatorWorker> logger, ServiceBusEmulatorHost emulator)
        {
            _logger = logger;
            _emulator = emulator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _emulator.StartAsync();
            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            await _emulator.StopAsync();
        }
    }
}