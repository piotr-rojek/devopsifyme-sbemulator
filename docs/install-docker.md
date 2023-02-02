# Installation using Docker

## Connection String - Azure Service Bus
> "Endpoint=sb://devopsifyme-local.servicebus.windows.net/;SharedAccessKeyName=all;SharedAccessKey=CLwo3FQ3S39Z4pFOQDefaiUd1dSsli4XOAj3Y9Uh1E=;EnableAmqpLinkRedirect=false"

Note that the connection string has to be like this because:
* Service Bus library validates if hostname is a valid Azure namespace,
* It has to match both /etc/hosts entry, and server certificate of our emulator

## Run from DockerHub

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

## Run from sources

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

