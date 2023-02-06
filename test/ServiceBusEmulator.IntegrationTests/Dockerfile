FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

COPY ./docker/rabbitmq-amqp1/testca/cacert.pem /usr/local/share/ca-certificates/cacert.crt
RUN update-ca-certificates

WORKDIR /app
COPY ./test/ServiceBusEmulator.IntegrationTests ./

RUN dotnet publish -c Release -o /app/out

WORKDIR /app/out

ENTRYPOINT ["dotnet", "test", "ServiceBusEmulator.IntegrationTests.dll"]