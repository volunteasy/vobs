package tests

import (
	"context"
	"crypto/rand"
	"database/sql"
	"fmt"
	"math"
	"math/big"
)

type executor interface {
	ExecContext(ctx context.Context, sql string, arguments ...interface{}) (sql.Result, error)
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