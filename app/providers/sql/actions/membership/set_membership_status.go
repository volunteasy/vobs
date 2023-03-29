package membership

import (
	"context"

	"govobs/app/core/membership"
	"govobs/app/providers/sql/query"
	"govobs/app/providers/sql/transaction"
)

func (a actions) SetMembershipStatus(ctx context.Context, m membership.Membership) error {
	const script = `update memberships set status = ? where org_id = ? and user_id = ?`

	exec, err := transaction.FromContext(ctx).ExecContext(ctx, script, m.Status, m.OrgID, m.UserID)
	if err != nil {
		return query.HandleDatabaseError(err, map[uint16]error{})
	}

	return query.HandleExecSingleResult(exec, membership.ErrNotFound)
}
