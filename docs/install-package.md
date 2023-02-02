# Installation from Package

First download the latest release from GitHub then adjust appsettings.json. Then update your connection string in the client application. At the end make sure that all prerequisites are met and start the emulator.

## Client Connection String

Note that the hostname has to end with ".servicebus.windows.net", as well as SharedAccess values must be exactly as specified.

> "Endpoint=sb://devopsifyme-local.servicebus.windows.net/;SharedAccessKeyName=all;SharedAccessKey=CLwo3FQ3S39Z4pFOQDefaiUd1dSsli4XOAj3Y9Uh1E=;EnableAmqpLinkRedirect=false"

## Update /etc/host
Redirect your hostname to the host you have specified in the connection string

## Server Certificate
Generate a server certificate to match your desired connection string hostname, ending with ".servicebus.windows.net". Afterwards import to your users local certificate store or save on disk as pfx.

## Install Rabbit MQ
Install or locate already existing server

## Update appsettings.json

Add following section to appsettings.json and make sure it is correct :)

```json
"Emulator": {
    "ServerCertificatePath": "/app/cert.pfx",
    "ServerCertificatePassword": "password",
    "Port": 5671,
    "RabbitMq": {
      "Username": "user",
      "Password": "password",
      "Host": "localhost",
      "Port": 5672
    }
  }
```

