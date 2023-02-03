# Installation using Docker

## Connection String - Azure Service Bus

Note that the SharedAccess values must be exactly as specified as this is currently hardcoded in the emulator.

> "Endpoint=sb://localhost/;SharedAccessKeyName=all;SharedAccessKey=CLwo3FQ3S39Z4pFOQDefaiUd1dSsli4XOAj3Y9Uh1E=;EnableAmqpLinkRedirect=false"

## Run from DockerHub

``` powershell
# Bring containers up
curl https://raw.githubusercontent.com/piotr-rojek/devopsifyme-sbemulator/main/docker-compose.yml?token=GHSAT0AAAAAAB46BQ33G7YMU6VVCKW55WXOY6ZRE2A --output docker-compose.yml
docker compose up --detach --no-build 

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

# Trust our test certificate
Import-Certificate -FilePath "docker\rabbitmq-amqp1\testca\cacert.cer" -CertStoreLocation cert:\CurrentUser\Root
```

