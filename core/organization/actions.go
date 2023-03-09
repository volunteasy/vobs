package organization

import (
	"context"
	"govobs/core/types"
)

type Actions interface {
	CreateOrganization(ctx context.Context, o Organization) error
	GetOrganization(ctx context.Context, id types.ID) (Organization, error)
	ListOrganizations(ctx context.Context, f Filter) ([]Organization, int, error)
	ListEnrollments(ctx context.Context, userID types.ID, f Filter) ([]Enrollments, error)
}
