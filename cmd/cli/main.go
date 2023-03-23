package main

import (
	"errors"
	"govobs/config"
	"govobs/providers/sql"
	"log"
	"os"
	"strconv"

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

	db, _, err := sql.NewConnection(cfg.MySQL)
	if err != nil {
		log.Fatal(err)
	}

	mig, err := sql.MigrationHandler(db, cfg.MySQL)
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
