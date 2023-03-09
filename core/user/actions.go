package user

import (
	"context"
	"govobs/core/types"
)

type Actions interface {
	CreateUser(ctx context.Context, u User) error
	ListUsers(ctx context.Context, f Filter) ([]User, int, error)
	GetUser(ctx context.Context, id types.ID) (User, error)
}
