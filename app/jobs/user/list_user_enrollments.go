package user

import (
	"context"

	"govobs/app/core/organization"
	"govobs/app/core/types"
)

func (j jobs) ListUserEnrollments(ctx context.Context, userID types.UserID, f organization.Filter) ([]organization.Enrollment, int, error) {
	if err := f.Validate(); err != nil {
		return nil, 0, err
	}

	return j.organizations.ListEnrollments(ctx, userID, f)
}
