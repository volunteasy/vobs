package main

import (
	"govobs/api"
	"govobs/config"
	"govobs/obs"
	"govobs/providers/sql"
	"log"
	"net"
	"time"

	"github.com/kelseyhightower/envconfig"
	"github.com/sirupsen/logrus"

	_ "github.com/joho/godotenv/autoload"
)

func main() {

	var cfg config.Config
	err := envconfig.Process("", &cfg)
	if err != nil {
		log.Fatal(err)
	}

	_, migrate, err := sql.NewConnection(cfg.MySQL)
	if err != nil {
		log.Fatal(err)
	}

	err = migrate()
	if err != nil {
		log.Fatal(err)
	}

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
