package user

import (
	"context"
	"govobs/app/core/benefit"
	"govobs/app/core/types"
)

func (j jobs) ListUserBenefits(ctx context.Context, userID types.UserID, f benefit.Filter) ([]benefit.Benefit, int, error) {
	f.AssistedID = userID
	if err := f.Validate(); err != nil {
		return nil, 0, err
	}

	return j.benefits.ListBenefits(ctx, benefit.Filter{
		ClaimDateRange: f.ClaimDateRange,
		AssistedID:     f.AssistedID,
		Filter:         f.Filter,
	})
}
