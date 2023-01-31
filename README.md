# DevOpsifyMe - Azure Service Bus Emulator

Emulator is really an AMQP 1.0 proxy that translates Azure Service Bus specifics into backend implementation specifics. Backend is an actual queue implementation that runs locally, not in cloud, and handles the messages. The main use case is to help with local development experience and with integration testing where application code cannot change driver easily.

At the moment following backends are supported:

* RabbitMQ (AMQP 0.9)
* InMemory (only simple message pipe)

Other backends that might get implemented:
* Azure Storage Queue (with local Azurite emulator)
* Apache ActiveMQ (also local, supports AMQP 1.0)

### Supported Features

Current implementation is considered experimental, but from initial tests it works pretty well with standard Azure Functions triggers and outputs. So messages are going through queues, topics and subscriptions. Following features are supported:

* Sending, Receiving, Peeking messages, including metadata
* Message Annotations: TTL, ID, Header, ApplicationData, and more
* Queues, Topics and Subscriptions
* Renew Locks (dummy)

### Not Supported Features

* Dead Letter (not implemented yet)
* Message Annotations: partitions, groups, reply-to and more (ignored)
* Sessions, Transactions
* Scheduled and Delayed messages
* Rules
* Any AMQP Draft Extensions
* Any Advanced Flow Control
* Not for production use, ever

## Installation

* Add "devopsifyme-local.servicebus.windows.net 127.0.0.1" to /etc/hosts
* Add /docker/testca/cacert.cer to Trusted Root on your local machine
* Start RabbitMQ container, or use any other installation you might already have
* Start ServiceBusEmulator, change configuration if needed
* Update connection string in your application :)

### Run from DockerHub
``` powershell
# Bring containers up
curl https://raw.githubusercontent.com/piotr-rojek/devopsifyme-sbemulator/main/docker-compose.yml?token=GHSAT0AAAAAAB46BQ33G7YMU6VVCKW55WXOY6ZRE2A --output docker-compose.yml
docker compose up --detach --no-build 

# Patch /etc/hosts
Add-Content -Path "C:\Windows\System32\drivers\etc\hosts" -value "devopsifyme-local.servicebus.windows.net 127.0.0.1"

# Trust our test certificate
$containerPrefix = Split-Path (Get-Location) -Leaf
docker cp $containerPrefix-emulator-1:/app/cacert.cer cacert.cer
Import-Certificate -FilePath cacert.cer -CertStoreLocation cert:\CurrentUser\Root
```

### Run from sources

``` powershell
# Clone repository
git clone https://github.com/piotr-rojek/devopsifyme-sbemulator.git

cd devopsifyme-sbemulator

# Bring containers up
docker compose build
docker compose up --detach

# Patch /etc/hosts
Add-Content -Path "C:\Windows\System32\drivers\etc\hosts" -value "devopsifyme-local.servicebus.windows.net 127.0.0.1"

# Trust our test certificate
Import-Certificate -FilePath "docker\rabbitmq-amqp1\testca\cacert.cer" -CertStoreLocation cert:\CurrentUser\Root
```

### Connection String - Azure Service Bus
> "Endpoint=sb://devopsifyme-local.servicebus.windows.net/;SharedAccessKeyName=all;SharedAccessKey=CLwo3FQ3S39Z4pFOQDefaiUd1dSsli4XOAj3Y9Uh1E=;EnableAmqpLinkRedirect=false"

Note that the connection string has to be like this because:
* Service Bus library validates if hostname is a valid Azure namespace,
* It has to match both /etc/hosts entry, and server certificate of our emulator

## Example - Azure Function

This example shows that a vanilla, unmodified Azure Function, running isolated process can use the emulator. Implemented scenario:
* Queue trigger handles message, sends a new one to a Topic
* Topic has two Subscriptions, each has a function trigger attached

## Example - No Proxy App

This example shows how to 'patch' an application to talk DIRECTLY to RabbitMQ with AQMP 1.0 enabled. To make it work, it required IL level patching of Microsoft provided 'Azure.Messaging.ServiceBus' nuget, so please consider this just a demonstration for educational purposes and not a real maintainable solution :) 

It also has its drawbacks and does support queues only at the moment (no topics or subscriptions). Moreover it requires queues to be created beforehand, as non-durable and consumer cannot ACK afterwards (MUST allow deletion on consumption).
