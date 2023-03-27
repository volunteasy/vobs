package organization

import (
	"context"

	"govobs/app/core/organization"
	"govobs/app/core/types"
)

func (a actions) GetOrganization(ctx context.Context, id types.ID) (o organization.Organization, err error) {
	const script = `
		select id, name, document, phone, address from organizations 
		where id = $1
	`

	err = a.db.QueryRowContext(ctx, script, id).
		Scan(&o.ID, &o.Name, &o.Document, &o.Contact.Address, &o.Contact.Address)

	if err != nil {
		// TODO: check err
		return organization.Organization{}, err
	}

	return o, nil
}
