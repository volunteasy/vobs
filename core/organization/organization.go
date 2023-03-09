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

	Enrollments struct {
		Organization
		Status membership.Status
		Role   membership.Role
	}
)
