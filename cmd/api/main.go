package main

import (
	"github.com/newrelic/go-agent/v3/newrelic"
	"github.com/sirupsen/logrus"
	"govobs/api"
	"govobs/telemetry"
	"log"
	"net"
	"time"
)

func main() {

	//var c config.Config
	//err := envconfig.Process("", &c)
	//if err != nil {
	//	log.Fatalf("could not get config variables from environment: %s", err)
	//}

	if "" == "production" {
		logrus.SetFormatter(&logrus.JSONFormatter{})
	}

	telemetry.NewLogger(logrus.WithFields(logrus.Fields{
		"app": "govobs",
		"env": "development",
	}))

	nr, err := newrelic.NewApplication(
		newrelic.ConfigAppName("govobs"),
		newrelic.ConfigLicense("28737fda66a2e862ce3cb63165b744913c45NRAL"),
		newrelic.ConfigAppLogForwardingEnabled(true),
	)

	if err != nil {
		log.Fatalf("failed initializing observability service: %s", err)
	}

	telemetry.NewTracer(nr)

	lis, err := net.Listen("tcp", ":8080")
	if err != nil {
		log.Fatalf("could not listen to port %s: %s", ":8080", err)
	}

	api.Serve(lis, api.Handler(), time.Second*30, time.Second*30)
}
