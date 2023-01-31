using Amqp;
using ServiceBusEmulator;
using ServiceBusEmulator.RabbitMq;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        _ = services.AddServiceBusEmulator();
        _ = services.AddServiceBusEmulatorRabbitMqBackend();
    })
    .Build();

Trace.TraceLevel = TraceLevel.Frame;
Trace.TraceListener = (l, f, a) => Console.WriteLine(DateTime.Now.ToString("[hh:mm:ss.fff]") + " " + string.Format(f, a));

host.Run();
