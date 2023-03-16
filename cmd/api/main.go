package main

import (
	"github.com/getsentry/sentry-go"
	"github.com/kelseyhightower/envconfig"
	"govobs/config"
	"log"
)

func main() {

	var c config.Config
	err := envconfig.Process("", &c)
	if err != nil {
		log.Fatalf("could not get config variables from environment: %s", err)
	}

	err = sentry.Init(sentry.ClientOptions{
		Dsn: c.Logger.DSN,
	})

	if err != nil {
		log.Fatalf("failed initializing observability service: %s", err)
	}
}
