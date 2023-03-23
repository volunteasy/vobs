package user

import (
	"context"
	"govobs/core/types"
	"govobs/core/user"
)

func (a actions) ListEnrolledUsers(ctx context.Context, orgID types.ID, f user.Filter) ([]user.Enrolled, int, error) {
	const script = `
        select 
            u.id,
            u.external_id,
            u.document,
            u.name,
            u.nickname
        from users u
        inner join memberships m
            on m.user_id = u.id
        where 
            m.org_id = $1
    `

	return nil, 0, nil
}
