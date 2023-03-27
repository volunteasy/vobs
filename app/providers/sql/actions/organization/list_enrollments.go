package organization

import (
	"context"

	"govobs/app/core/organization"
	"govobs/app/core/types"
	"govobs/app/providers/sql/query"
)

func (a actions) ListEnrollments(ctx context.Context, userID types.UserID, f organization.Filter) ([]organization.Enrollment, int, error) {
	const script = `
		select 
		    o.id, o.name, o.document, o.phone, o.address, m.status, m.role 
		from organizations o
		inner join memberships m 
		    on o.id = m.org_id
	`

	q := query.NewQuery(script).
		Where(true, "m.user_id = $%d", userID).And().
		Where(f.Name != "", "o.name like $%d", "%"+f.Name+"%").
		Limit(f.Range)

	rows, err := a.db.QueryContext(ctx, q.Query(), q.Args...)
	if err != nil {
		return nil, 0, err
	}

	orgs := make([]organization.Enrollment, 0)
	for o := (organization.Enrollment{}); rows.Next(); {
		if err := rows.Scan(
			&o.ID, &o.Name, &o.Document, &o.Contact.Address,
			&o.Contact.Address, &o.Status, &o.Role,
		); err != nil {
			return nil, 0, err
		}

		orgs = append(orgs, o)
	}

	count := len(orgs)
	if count >= f.Range.Limit()+1 {
		return orgs[0 : count-1], count, nil
	}

	return orgs, count, nil
}
