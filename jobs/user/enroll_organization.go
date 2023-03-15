package user

import (
    "context"
    "govobs/core/membership"
)

func (j jobs) EnrollOrganization(ctx context.Context, m membership.Membership) (membership.Membership, error) {
	if err := m.Validate(); err != nil {
		return membership.Membership{}, err
	}

	return m, j.membership.CreateMembership(ctx, m)
}