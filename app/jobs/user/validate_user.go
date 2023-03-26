package user

import (
	"context"
	"errors"
	"govobs/app/core/user"
)

func (j jobs) ValidateUser(ctx context.Context, u user.User) error {
	if err := u.Validate(); err != nil {
		return err
	}

	_, err := j.users.GetUserWithDocument(ctx, u.Document)
	if err != nil && !errors.Is(err, user.ErrNotFound) {
		return err
	}

	return nil
}
