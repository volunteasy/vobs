package user

import (
	"context"
	"govobs/core/user"
)

func (j jobs) CreateUser(ctx context.Context, u user.User) (user.User, error) {
	if err := u.Validate(); err != nil {
		return user.User{}, err
	}

	u.ID = j.createID()

	err := j.users.CreateUser(ctx, u)
	if err != nil {
		return user.User{}, err
	}

	return u, nil
}
