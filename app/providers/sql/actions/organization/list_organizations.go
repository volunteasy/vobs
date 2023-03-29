package organization

import (
	"context"
	"encoding/json"

	"govobs/app/core/organization"
	"govobs/app/providers/sql/query"
)

func (a actions) ListOrganizations(ctx context.Context, f organization.Filter) ([]organization.Organization, int, error) {
	const script = `
		select 
		    o.id, o.name, o.document, o.phone, o.address 
		from organizations o
	`

	q := query.NewQuery(script).
		Where(f.Name != "", "o.name like ?", "%"+f.Name+"%").
		Limit(f.Range)

	rows, err := a.db.QueryContext(ctx, q.Query(), q.Args...)
	if err != nil {
		return nil, 0, err
	}

	orgs := make([]organization.Organization, 0)
	for o := (organization.Organization{}); rows.Next(); {

		var address string
		if err := rows.Scan(
			&o.ID, &o.Name, &o.Document, &o.Contact.Phone,
			&address,
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
	if count >= f.Range.Limit()+1 {
		return orgs[0 : count-1], count, nil
	}

	return orgs, count, nil
}
