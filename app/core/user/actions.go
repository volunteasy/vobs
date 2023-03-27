package user

import (
	"context"

	"govobs/app/core/types"
)

//go:generate moq -fmt goimports -out actions_mock.go . Actions:ActionsMock

type Actions interface {
	GetUser(ctx context.Context, id types.UserID) (User, error)
	GetUserWithDocument(ctx context.Context, doc types.Document) (User, error)
}
