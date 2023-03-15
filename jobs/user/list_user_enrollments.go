package user

import (
    "context"
    "govobs/core/organization"
    "govobs/core/types"
)

func (j jobs) ListUserEnrollments(ctx context.Context, userID types.ID, f organization.Filter) ([]organization.Enrollment, int, error) {
	if err := f.Validate(); err != nil {
		return nil, 0, err
	}

	return j.organizations.ListEnrollments(ctx, userID, f)
}