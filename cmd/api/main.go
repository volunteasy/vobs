package main

import (
	"govobs/api"
	"govobs/app"
	"govobs/config"
	"govobs/providers/sql"
	"log"
	"net"
	"time"

	"github.com/kelseyhightower/envconfig"
	"github.com/sirupsen/logrus"

	_ "github.com/joho/godotenv/autoload"
)

// @title						GOVOBS - Golang Volunteasy Backend Service
// @version					1.0
// @securityDefinitions.apikey	AuthKey
// @in							header
// @name						Authorization
func main() {

	var cfg config.Config
	err := envconfig.Process("", &cfg)
	if err != nil {
		log.Fatal(err)
	}

	db, migrate, err := sql.NewConnection(cfg.MySQL)
	if err != nil {
		log.Fatal(err)
	}

	err = migrate()
	if err != nil {
		log.Fatal(err)
	}

	if cfg.Environment == "production" {
		logrus.SetFormatter(&logrus.JSONFormatter{})
	}

	app, err := app.NewApp(app.Deps{
		DB: db,
		Logger: logrus.WithFields(logrus.Fields{
			"app": "govobs",
			"env": cfg.Environment,
		}),
	}, cfg)

	if err != nil {
		log.Fatalf("could not initialize application: %s", err)
	}

	lis, err := net.Listen("tcp", ":8080")
	if err != nil {
		log.Fatalf("could not listen to port %s: %s", ":8080", err)
	}

	api.NewServer(
		time.Second*30,
		time.Second*30,
		app.Logger,
	)(lis,
		api.Handler(app),
	)
}
