FROM debian:stable-slim 

RUN apt-get update -y \
    && DEBIAN_FRONTEND=noninteractive apt-get install locales tzdata openssl ca-certificates -y \
    && /usr/sbin/update-ca-certificates \
    && locale-gen en_US.UTF-8 pt_BR.UTF-8

ENV	LANG=en_US.UTF-8 \
    LANGUAGE=en_US:en \
    LC_ALL=en_US.UTF-8 \
    DEBIAN_FRONTEND=noninteractive

ADD build/volunteasy-api /bin/

ADD build/volunteasy-cli /bin/

ENTRYPOINT [ "/bin/volunteasy-api" ]

