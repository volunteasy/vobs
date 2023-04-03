package organization

import (
	"context"
	"encoding/json"

	"govobs/app/core/organization"
	"govobs/app/core/types"
	"govobs/app/providers/mysql/conn/query"
)

func (a actions) ListEnrollments(ctx context.Context, userID types.ID, f organization.Filter) ([]organization.Enrollment, int, error) {
	const script = `
		select 
		    o.id, o.name, o.document, o.phone, o.address, m.status, m.role 
		from organizations o
		inner join memberships m 
		    on o.id = m.org_id
	`

	q := query.NewQuery(script).
		Where(true, "m.user_id = ?", userID).And().
		Where(f.Name != "", "o.name like ?", "%"+f.Name+"%").
		Limit(f.Range)

	rows, err := a.db.QueryContext(ctx, q.Query(), q.Args...)
	if err != nil {
		return nil, 0, err
	}

	orgs := make([]organization.Enrollment, 0)
	for o := (organization.Enrollment{}); rows.Next(); {

		var address string
		if err := rows.Scan(
			&o.ID, &o.Name, &o.Document, &o.Contact.Phone,
			&address, &o.Status, &o.Role,
		); err != nil {
			return nil, 0, err
		}

		err = json.Unmarshal([]byte(address), &o.Contact.Address)
		if err != nil {
			return nil, 0, err
		}

		orgs = append(orgs, o)
	}

	count := len(orgs)
	if count >= f.Range.Limit() {
		return orgs[0 : count-1], count, nil
	}

	return orgs, count, nil
}
