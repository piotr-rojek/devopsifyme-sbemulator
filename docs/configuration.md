# Configuration

Environmental Variable | Docker Default | Description
-|-|-
Emulator__QueuesAndTopics | | Coma-separated list of queues and topics to be created on startup. See below
Emulator__Port | 5671 | Port on which service is listening
Emulator__ServerCertificatePath | /app/cert.pfx | Path to a PFX file containing emulator's TLS certificate
Emulator__ServerCertificatePassword | password | Password for the PFX file
Emulator__ServerCertificateThumbprint |  | Server certificate in current user's certificate store
Emulator__RabbitMq__Username | guest | RabbitMQ backend - username
Emulator__RabbitMq__Password | guest | RabbitMQ backend - password
Emulator__RabbitMq__Host | localhost | RabbitMQ backend - hostname
Emulator__RabbitMq__Port | 5672 | RabbitMQ backend - port

## Emulator.QueuesAndTopics

You can use this to preinitialize queues and topics, if your tests require this. Otherwise some messages sent might be lost, if the receiver connects after they are sent. For example, integration tests in this repo rely on this setting (see docker-compose.integration.yml).

Sample value
```
queue-name;topic-name/Subscriptions/sub1;topic-name/Subscriptions/sub2
```

## Bring your own certificate

If you don't want to trust provided test CA, you can provide your own server certificate. Mount your PFX file in any directory, and update environmental variables `Emulator__ServerCertificatePath` and `Emulator__ServerCertificatePassword`.

> Note that the provided server certificate is regenerated each release, so it is not convenient to trust it. Therefore by default it is required to trust the Test CA from this repository. Since this certificate is publicly available here, it could potentially allow attacker to generate any certificate that you would trust.