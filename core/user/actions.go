package user

import (
	"context"
	"govobs/core/types"
)

type Actions interface {
	CreateUser(ctx context.Context, u User) error
	ListEnrolledUsers(ctx context.Context, f Filter) ([]Enrolled, int, error)
	GetUser(ctx context.Context, id types.ID) (User, error)
}
