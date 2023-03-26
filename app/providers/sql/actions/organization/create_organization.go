package organization

import (
	"context"
	"encoding/json"
	"govobs/app/core/organization"
	"govobs/app/providers/sql/query"
	"govobs/app/providers/sql/transaction"
)

func (a actions) CreateOrganization(ctx context.Context, o organization.Organization) error {
	const script = ` 
		insert into organizations (id, name, document, phone, address) values (
			?, ?, ?, ?, ?                                                                       
		);
	`

	byarr, err := json.Marshal(o.Contact.Address)
	if err != nil {
		return err
	}

	_, err = transaction.FromContext(ctx).ExecContext(ctx, script,
		o.ID, o.Name, o.Document, o.Contact.Phone, string(byarr),
	)

	return query.HandleDatabaseError(err, map[uint16]error{
		1062: organization.ErrAlreadyExists,
	})
}
