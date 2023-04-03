package conn

import (
	"database/sql"
	"fmt"

	"govobs/app/config"

	"github.com/go-sql-driver/mysql"
)

func NewConnection(c config.MySQL) (*sql.DB, error) {
	cfg, err := mysql.ParseDSN(c.DSN)
	if err != nil {
		return nil, fmt.Errorf("failed creating mysql configurations: %w", err)
	}

	cfg.InterpolateParams = true
	cfg.MultiStatements = true

	conn, err := mysql.NewConnector(cfg)
	if err != nil {
		return nil, fmt.Errorf("failed creating mysql connector: %w", err)
	}

	db := sql.OpenDB(conn)

	return db, db.Ping()
}
