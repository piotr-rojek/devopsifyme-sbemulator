FROM rabbitmq:3.8-management

RUN apt-get update \
	&& apt-get install openssl -y  \ 
    && mkdir -p /home \
    && mkdir -p /home/testca \
	&& mkdir -p /home/testca/certs \
	&& mkdir -p /home/testca/private \
	&& chmod 777 -R /home \
	&& echo 01 > /home/testca/serial \
	&& touch /home/testca/index.txt

COPY testca/ /home/testca

COPY rabbitmq.conf /etc/rabbitmq/rabbitmq.conf
COPY openssl.cnf /home/testca/
COPY prepare-server.sh /home/

RUN rabbitmq-plugins enable rabbitmq_amqp1_0

RUN mkdir -p /home/server \
	&& mkdir -p /home/client \
	&& chmod +x /home/prepare-server.sh

RUN /bin/bash /home/prepare-server.sh