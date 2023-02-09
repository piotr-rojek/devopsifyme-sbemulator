FROM --platform=amd64 alpine/openssl:latest AS server-cert

    WORKDIR /home/testca
    COPY ./docker/rabbitmq-amqp1/testca .
    COPY ./docker/rabbitmq-amqp1/openssl.cnf .
    RUN mkdir certs && \
        echo 01 > /home/testca/serial && \
	    touch /home/testca/index.txt && \
        openssl genrsa -out key.pem 2048 && \
        openssl req -new -key key.pem -out req.pem -outform PEM -nodes \
          -subj /CN=devopsifyme-local.servicebus.windows.net/O=server/ \
          -config openssl.cnf \
          -addext "subjectAltName = DNS:localhost, DNS:sbemulator, DNS:emulator" && \
        openssl ca -config openssl.cnf -in req.pem -out cert.pem -notext -batch -extensions server_ca_extensions && \
        openssl pkcs12 -export -out cert.pfx -inkey key.pem -in cert.pem -passout pass:password

FROM --platform=amd64 mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

    WORKDIR /app
    COPY . ./
    RUN dotnet test --filter FullyQualifiedName\!~IntegrationTests

    WORKDIR /app/src/ServiceBusEmulator.Host
    RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/runtime:7.0

    ENV DOTNET_ENVIRONMENT=Docker

    WORKDIR /app
    COPY --from=build-env /app/out .
    COPY --from=server-cert /home/testca/cert.pfx .
    COPY --from=server-cert /home/testca/cacert.cer .

    ENTRYPOINT ["dotnet", "ServiceBusEmulator.Host.dll"]