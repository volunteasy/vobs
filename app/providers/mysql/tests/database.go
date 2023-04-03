package tests

import (
	"context"
	sq "database/sql"
	"govobs/app/config"
	"govobs/app/providers/mysql/conn"
	"testing"
)

func NewDatabase(t *testing.T) *sq.DB {
	t.Helper()

	co, err := conn.NewConnection(config.MySQL{
		DSN: dsn,
	})

	if err != nil {
		t.Fatalf("Could not create db connection for test %s: %v", t.Name(), err)
	}

	dbname, ctx := createName(), context.Background()

	err = createDatabase(context.Background(), co, dbname)
	if err != nil {
		t.Fatalf("Could not create database for test %s: %v", t.Name(), err)
	}

	t.Cleanup(func() {
		removeDatabase(ctx, co, dbname)
		co.Close()
	})

	cfg := config.MySQL{
		DSN:         dsn + dbname,
		AddTestData: true,
	}

	db, err := conn.NewConnection(cfg)
	if err != nil {
		t.Fatalf("Could not connect to database for test %s: %v", t.Name(), err)
	}

	t.Cleanup(func() {
		db.Close()
	})

	err = conn.MigrateDatabase(db, cfg.AddTestData)
	if err != nil {
		t.Fatalf("Could not migrate database for test %s: %v", t.Name(), err)
	}

	return db
}
