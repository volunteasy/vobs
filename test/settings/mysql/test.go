package mysql

import (
	"context"
	"database/sql"
	"govobs/config"
	conn "govobs/providers/sql"
	"testing"
)

type DatabaseContextBuilder func(t *testing.T) *sql.DB

type Callback = func(ctx context.Context, t *testing.T)

func builder(c *sql.DB, port string) DatabaseContextBuilder {
	return func(t *testing.T) *sql.DB {
		t.Helper()

		test, name := t.Name(), createName()

		err := createDatabase(context.Background(), c, name)
		if err != nil {
			t.Errorf("Could not create database for test %s: %v", test, err)
			t.FailNow()
		}

		db, mig, err := conn.NewConnection(config.MySQL{
			Host:     "localhost:" + port,
			Name:     name,
			User:     "volunteasy",
			Password: "volunteasy",
		})

		if err != nil {
			t.Errorf("Could not connect to database for test %s: %v", test, err)
			t.FailNow()
		}

		err = mig()
		if err != nil {
			t.Errorf("Could not migrate database for test %s: %v", test, err)
			t.FailNow()
		}

		return db
	}
}

func (b DatabaseContextBuilder) TX(t *testing.T) *sql.Tx {
	t.Helper()

	tx, _ := b(t).Begin()
	return tx
}
