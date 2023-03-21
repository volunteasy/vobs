package organization

import (
	"context"
	"govobs/core/distribution"
	"govobs/core/organization"
	"govobs/core/types"
)

type (
	Jobs interface {
		CreateOrganization(ctx context.Context, org organization.Organization, ownerID types.ID) (organization.Organization, error)
		GetOrganization(ctx context.Context, id types.ID) (organization.Organization, error)
		UpdateOrganization(ctx context.Context, org organization.Organization) error
		RemoveOrganization(ctx context.Context, id types.ID) error

		CreateDistribution(ctx context.Context, d distribution.Distribution) (distribution.Distribution, error)
		ListDistributions(ctx context.Context, orgID types.ID, f distribution.Filter) ([]distribution.Distribution, int, error)
		GetDistribution(ctx context.Context, id types.ID) (distribution.Distribution, error)
		UpdateDistribution(ctx context.Context, d distribution.Distribution) error
		RemoveDistribution(ctx context.Context, id types.ID) error
	}
)
