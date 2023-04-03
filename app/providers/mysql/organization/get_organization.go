package organization

import (
	"context"
	"encoding/json"

	"govobs/app/core/organization"
	"govobs/app/core/types"
	"govobs/app/providers/mysql/conn/query"
)

func (a actions) GetOrganization(ctx context.Context, id types.ID) (o organization.Organization, err error) {
	const script = `
		select id, name, document, phone, address from organizations 
		where id = ?
	`
	var address string
	err = a.db.QueryRowContext(ctx, script, id).
		Scan(&o.ID, &o.Name, &o.Document, &o.Contact.Phone, &address)

	if err != nil {
		return organization.Organization{}, query.HandleNotFoundErr(err, organization.ErrNotFound)
	}

	err = json.Unmarshal([]byte(address), &o.Contact.Address)
	if err != nil {
		return organization.Organization{}, err
	}

	return o, nil
}
