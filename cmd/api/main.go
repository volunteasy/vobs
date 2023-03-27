package main

import (
	"log"
	"net"
	"time"

	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/credentials"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/cognitoidentityprovider"
	"github.com/bwmarrin/snowflake"
	_ "github.com/joho/godotenv/autoload"
	"github.com/kelseyhightower/envconfig"
	"github.com/sirupsen/logrus"
	"govobs/app"
	"govobs/app/api"
	"govobs/app/config"
	"govobs/app/providers/sql"
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

	if cfg.Environment == "production" {
		logrus.SetFormatter(&logrus.JSONFormatter{})
	}

	db, migrate, err := sql.NewConnection(cfg.MySQL)
	if err != nil {
		log.Fatal(err)
	}

	err = migrate()
	if err != nil {
		log.Fatal(err)
	}

	creds := credentials.NewCredentials(&credentials.StaticProvider{
		Value: credentials.Value{
			AccessKeyID:     cfg.AWS.AccessKeyID,
			SecretAccessKey: cfg.AWS.AccessKeySecret,
		},
	})

	sess, err := session.NewSession(&aws.Config{
		Credentials: creds,
		Region:      aws.String(cfg.AWS.Region),
	},
	)
	if err != nil {
		log.Fatal(err)
	}

	snnode, err := snowflake.NewNode(107)
	if err != nil {
		log.Fatal(err)
	}

	app, err := app.NewApp(app.Deps{
		DB:      db,
		IDNode:  snnode,
		Cognito: cognitoidentityprovider.New(sess),
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
