using Amqp;
using ServiceBusEmulator.RabbitMq;
using Xim.Simulators.ServiceBus;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddServiceBusEmulator();
        services.AddServiceBusEmulatorRabbitMqBackend();
    })
    .Build();

Trace.TraceLevel = TraceLevel.Frame;
Trace.TraceListener = (l, f, a) => Console.WriteLine(DateTime.Now.ToString("[hh:mm:ss.fff]") + " " + string.Format(f, a));

host.Run();
