package user

import (
	"context"
	"govobs/core/types"
)

//go:generate moq -fmt goimports -out actions_mock.go . Actions:ActionsMock

type Actions interface {
	CreateUser(ctx context.Context, u User) error
	ListEnrolledUsers(ctx context.Context, orgID types.ID, f Filter) ([]Enrolled, int, error)
	GetUser(ctx context.Context, id types.ID) (User, error)
}
