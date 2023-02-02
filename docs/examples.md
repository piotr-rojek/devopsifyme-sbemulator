# Examples

Look into /src/Example* folder for source code

## Azure Function

This example shows that a vanilla, unmodified Azure Function, running isolated process can use the emulator. Implemented scenario:
* Queue trigger handles message, sends a new one to a Topic
* Topic has two Subscriptions, each has a function trigger attached

![](example-azure-function.gif)

## No Proxy App

This example shows how to 'patch' an application to talk DIRECTLY to RabbitMQ with AQMP 1.0 enabled. To make it work, it required IL level patching of Microsoft provided 'Azure.Messaging.ServiceBus' nuget, so please consider this just a demonstration for educational purposes and not a real maintainable solution :) 

It also has its drawbacks and does support queues only at the moment (no topics or subscriptions). Moreover it requires queues to be created beforehand, as non-durable and consumer cannot ACK afterwards (MUST allow deletion on consumption).
