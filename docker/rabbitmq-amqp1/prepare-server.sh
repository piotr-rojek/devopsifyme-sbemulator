#!/bin/bash

set -eu

#
# Prepare the server's stuff.
#
cd /home/server

# Generate a private RSA key.
openssl genrsa -out key.pem 2048

# Generate a certificate from our private key.
openssl req -new -key key.pem -out req.pem -outform PEM -subj /CN=devopsifyme-local.servicebus.windows.net/O=server/ -config /home/testca/openssl.cnf -addext "subjectAltName = DNS:localhost" -nodes

# Sign the certificate with our CA.
cd /home/testca
openssl ca -config openssl.cnf -in /home/server/req.pem -out /home/server/cert.pem -notext -batch -extensions server_ca_extensions

# Create a key store that will contain our certificate.
cd /home/server
openssl pkcs12 -export -out keycert.p12 -in cert.pem -inkey key.pem -passout pass:roboconf

chmod -R 777 .