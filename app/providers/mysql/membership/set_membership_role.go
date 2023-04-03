package membership

import (
	"context"

	"govobs/app/core/membership"
	"govobs/app/providers/mysql/conn/query"
	"govobs/app/providers/mysql/conn/transaction"
)

func (a actions) SetMembershipRole(ctx context.Context, m membership.Membership) error {
	const script = `update memberships set role = ? where org_id = ? and user_id = ?`

	exec, err := transaction.FromContext(ctx).ExecContext(ctx, script, m.Role, m.OrgID, m.UserID)
	if err != nil {
		return query.HandleDatabaseError(err, map[uint16]error{})
	}

	return query.HandleExecSingleResult(exec, membership.ErrNotFound)
}
