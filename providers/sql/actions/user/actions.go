package user

import (
	"context"
	"database/sql"
	"govobs/core/types"
	"govobs/core/user"
)

type actions struct {
	db *sql.DB
}

func (a actions) GetUser(ctx context.Context, id types.ID) (user.User, error) {
	return user.User{}, nil
}

func NewActions() user.Actions {
	return actions{}
}
