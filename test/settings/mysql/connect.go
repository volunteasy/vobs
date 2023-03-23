package mysql

import (
	"context"
	"crypto/rand"
	"database/sql"
	"fmt"
	"govobs/config"
	conn "govobs/providers/sql"
	"math"
	"math/big"
)

type executor interface {
	ExecContext(ctx context.Context, sql string, arguments ...interface{}) (sql.Result, error)
}

func useConnection(cfg config.MySQL) (*sql.DB, func() error, error) {
	db, mig, err := conn.NewConnection(cfg)
	if err != nil {
		return nil, nil, err
	}

	return db, mig, db.Ping()
}

func createName() string {
	n, _ := rand.Int(rand.Reader, big.NewInt(math.MaxInt32))
	return fmt.Sprintf("database_%d", n)
}

func createDatabase(ctx context.Context, conn executor, name string) error {
	_ = removeDatabase(ctx, conn, name)

	if _, err := conn.ExecContext(context.Background(), fmt.Sprintf(`create database %s;`, name)); err != nil {
		return fmt.Errorf("failed creating test database %s: %w", name, err)
	}
	return nil
}

func removeDatabase(ctx context.Context, conn executor, name string) error {
	if _, err := conn.ExecContext(context.Background(), fmt.Sprintf(`drop database if exists %s;`, name)); err != nil {
		return fmt.Errorf("failed dropping test database %s: %w", name, err)
	}

	return nil
}
