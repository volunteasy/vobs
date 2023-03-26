package organization

import (
	"database/sql"
	"govobs/app/core/organization"
)

type actions struct {
	db *sql.DB
}

func NewActions(db *sql.DB) organization.Actions {
	return actions{
		db: db,
	}
}
