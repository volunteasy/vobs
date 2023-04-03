package membership

import (
	"context"

	"govobs/app/core/membership"
	"govobs/app/core/types"
	"govobs/app/providers/mysql/conn/query"
	"govobs/app/providers/mysql/conn/transaction"
)

func (a actions) RemoveMembership(ctx context.Context, userID types.UserID, organizationID types.ID) error {
	const script = `
		delete from memberships where org_id = ? and user_id = ?
	`

	exec, err := transaction.FromContext(ctx).ExecContext(ctx, script, organizationID, userID)
	if err != nil {
		return query.HandleDatabaseError(err, map[uint16]error{})
	}

	return query.HandleExecSingleResult(exec, membership.ErrNotFound)
}
