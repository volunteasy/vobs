package user

import (
	"context"
	"govobs/core/user"
	"govobs/providers/sql/transaction"
)

func (a actions) CreateUser(ctx context.Context, u user.User) error {
	const script = `
        insert 
            user (id, external_id, document, name, nickname)
        values (
            $1, $2, $3, $4, $5
        )
    `
	_, err := transaction.
		FromContext(ctx).
		ExecContext(ctx, script,
			u.ID,
			u.ExternalID,
			u.Document,
			u.Name,
			u.Nickname,
		)

	if err != nil {
		return err
	}

	return nil
}
