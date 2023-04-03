package main

import (
	"errors"
	"log"
	"os"
	"strconv"

	"govobs/app/config"
	"govobs/app/providers/mysql/conn"

	migrate "github.com/golang-migrate/migrate/v4"
	_ "github.com/joho/godotenv/autoload"
	"github.com/kelseyhightower/envconfig"
	cli "github.com/urfave/cli/v2"
)

func main() {
	var cfg config.Config
	err := envconfig.Process("", &cfg)
	if err != nil {
		log.Fatal(err)
	}

	db, err := conn.NewConnection(cfg.MySQL)
	if err != nil {
		log.Fatal(err)
	}

	mig, err := conn.MigrationHandler(db)
	if err != nil {
		log.Fatal(err)
	}

	defer mig.Close()

	app := &cli.App{
		Commands: []*cli.Command{
			{
				Name:    "migrate",
				Aliases: []string{"m"},
				Usage:   "migrate commands",
				Subcommands: []*cli.Command{
					{
						Name:  "up",
						Usage: "migrate up",
						Action: func(c *cli.Context) error {
							return mig.Up()
						},
					},
					{
						Name:  "down",
						Usage: "migrate down",
						Action: func(c *cli.Context) error {
							return mig.Down()
						},
					},
					{
						Name:  "force",
						Usage: "migrate force",
						Action: func(c *cli.Context) error {
							version, _ := strconv.Atoi(c.Args().First())
							return mig.Force(version)
						},
					},
					{
						Name:  "testdata",
						Usage: "adds mocked data to the database for all tables",
						Action: func(c *cli.Context) error {
							return conn.AddTestData(db)
						},
					},
				},
			},
		},
	}

	app.Name = "Govobs CLI"
	app.Usage = "Migration tooling and other features"

	err = app.Run(os.Args)
	if err != nil {
		switch {
		case errors.Is(err, migrate.ErrNoChange):
		case errors.Is(err, migrate.ErrNilVersion):
			log.Println(err.Error())
		default:
			log.Println(err.Error())
		}
	}
}
