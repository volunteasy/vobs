package membership

import (
	"database/sql"

	"govobs/app/core/membership"
)

type actions struct {
	db *sql.DB
}

func NewActions(db *sql.DB) membership.Actions {
	return actions{
		db: db,
	}
}
