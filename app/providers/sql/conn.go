package sql

import (
	"database/sql"
	"embed"
	"errors"
	"fmt"
	"net/http"

	"github.com/go-sql-driver/mysql"
	_ "github.com/go-sql-driver/mysql"
	"github.com/golang-migrate/migrate/v4"
	migratemysql "github.com/golang-migrate/migrate/v4/database/mysql"
	"github.com/golang-migrate/migrate/v4/source/httpfs"
	"govobs/app/config"
)

func NewConnection(c config.MySQL) (*sql.DB, func() error, error) {
	cfg, err := mysql.ParseDSN(c.DSN)
	if err != nil {
		return nil, nil, fmt.Errorf("failed creating mysql configurations: %w", err)
	}

	cfg.InterpolateParams = true
	cfg.MultiStatements = true

	conn, err := mysql.NewConnector(cfg)
	if err != nil {
		return nil, nil, fmt.Errorf("failed creating mysql connector: %w", err)
	}

	return sql.OpenDB(conn), func() error {
		migration, err := MigrationHandler(c)
		if err != nil {
			return err
		}

		defer migration.Close()

		if err := migration.Up(); err != nil && !errors.Is(err, migrate.ErrNoChange) {
			return fmt.Errorf("error executing migration: %w", err)
		}

		return nil
	}, nil
}

func MigrationHandler(c config.MySQL) (*migrate.Migrate, error) {
	db, err := sql.Open("mysql", c.DSN)
	if err != nil {
		return nil, fmt.Errorf("failed creating mysql connection for migration: %w", err)
	}

	source, err := httpfs.New(http.FS(migrationsFS), "migrations")
	if err != nil {
		return nil, fmt.Errorf("failed to create migration httpfs driver: %w", err)
	}

	d, err := migratemysql.WithInstance(db, &migratemysql.Config{})
	if err != nil {
		return nil, fmt.Errorf("failed to create mysql migration instance: %w", err)
	}

	migration, err := migrate.NewWithInstance("httpfs", source, "", d)
	if err != nil {
		return nil, fmt.Errorf("failed to create migration source instance: %w", err)
	}

	return migration, nil
}

//go:embed migrations
var migrationsFS embed.FS
