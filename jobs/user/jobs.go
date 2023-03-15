package user

import (
	"context"
	"govobs/core/organization"
	"govobs/core/types"
	"govobs/core/user"
)

type (
	Jobs interface {
		CreateUser(ctx context.Context, u user.User) (user.User, error)
		GetUser(ctx context.Context, id types.ID) (user.User, error)
		CreateOrganization(ctx context.Context, userID types.ID, o organization.Organization) (organization.Organization, error)
		EnrollOrganization(ctx context.Context, userID types.ID, m user.Membership) error
		ListOrganizations(ctx context.Context, f organization.Filter) ([]organization.Organization, int, error)
	}
)
