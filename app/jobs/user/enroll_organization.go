package user

import (
	"context"

	"govobs/app/core/membership"
)

func (j jobs) EnrollOrganization(ctx context.Context, m membership.Membership) (membership.Membership, error) {
	m.Status = membership.StatusPending
	if m.Role == membership.RoleAssisted {
		m.Status = membership.StatusAccepted
	}

	if err := m.Validate(); err != nil {
		return membership.Membership{}, err
	}

	err := j.membership.CreateMembership(ctx, m)
	if err != nil {
		return membership.Membership{}, err
	}

	return m, nil
}
