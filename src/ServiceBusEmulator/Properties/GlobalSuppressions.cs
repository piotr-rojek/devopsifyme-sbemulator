
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.


[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0067:Dispose objects before losing scope", Justification = "Disposed in parent QueueEntity", Scope = "member", Target = "~M:Xim.Simulators.ServiceBus.Delivering.DeliveryQueue.Process(Amqp.Listener.MessageContext)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in parent QueueEntity", Scope = "member", Target = "~M:Xim.Simulators.ServiceBus.Delivering.DeliveryQueue.Process(Amqp.Listener.MessageContext)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by simulation", Scope = "member", Target = "~M:Xim.Simulators.ServiceBus.ServiceBusBuilder.Build~Xim.Simulators.ServiceBus.IServiceBusSimulator")]