package main

import (
	"govobs/api"
	"govobs/obs"
	"log"
	"net"
	"time"

	"github.com/sirupsen/logrus"
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

	logrusEntry := logrus.WithFields(logrus.Fields{
		"app": "govobs",
		"env": "development",
	})

	obs.NewLogger(logrusEntry)

	lis, err := net.Listen("tcp", ":8080")
	if err != nil {
		log.Fatalf("could not listen to port %s: %s", ":8080", err)
	}

	hdl := api.Handler(api.Deps{Logger: logrusEntry})

	srv := api.NewServer(time.Second*30, time.Second*30, logrusEntry)

	srv(lis, hdl)
}
