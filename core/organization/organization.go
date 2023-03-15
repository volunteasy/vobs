package organization

import (
	"govobs/core/membership"
	"govobs/core/types"
)

type (
	Organization struct {
		ID       types.ID
		Name     string
		Document types.Document
		Contact  types.Contact
	}

	Filter struct {
		Name string
		types.Filter
	}

	Enrollment struct {
		Organization
		Status membership.Status
		Role   membership.Role
	}
)

func (o Organization) Validate() error {
	if len(o.Name) == 0 || len(o.Name) > 50 {
		return ErrInvalidName
	}

	if err := o.Document.Validate(); err != nil {
		return err
	}

	if err := o.Contact.Validate(); err != nil {
		return err
	}

	return nil
}