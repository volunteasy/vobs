package user

import (
	"context"

	"govobs/app/core/types"
	"govobs/app/core/user"
)

func (j jobs) GetUser(ctx context.Context, id types.UserID) (user.User, error) {
	return j.users.GetUser(ctx, id)
}
