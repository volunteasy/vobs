package membership

import (
	"context"

	"govobs/app/core/membership"
	"govobs/app/providers/mysql/conn/query"
	"govobs/app/providers/mysql/conn/transaction"
)

func (a actions) CreateMembership(ctx context.Context, m membership.Membership) error {
	const script = `
		insert into memberships (user_id, org_id, role, status) values(
			?, ?, ?, ?
		)
	`

	_, err := transaction.FromContext(ctx).ExecContext(ctx, script, m.UserID, m.OrgID, m.Role, m.Status)
	return query.HandleDatabaseError(err, map[uint16]error{
		1062: membership.ErrAlreadyExists,
	})
}
