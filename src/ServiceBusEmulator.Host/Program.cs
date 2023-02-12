using Amqp;
using ServiceBusEmulator;
using ServiceBusEmulator.RabbitMq;

Trace.TraceLevel = TraceLevel.Frame;
Trace.TraceListener = (l, f, a) => Console.WriteLine(DateTime.Now.ToString("[hh:mm:ss.fff]") + " " + string.Format(f, a));

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServiceBusEmulator();
builder.Services.AddServiceBusEmulatorRabbitMqBackend();

var app = builder.Build();
app.UseHealthChecks("/health");
app.UseRouting();
app.Run();
