package user

import (
    "context"
    "govobs/core/benefit"
    "govobs/core/types"
)

func (j jobs) ListUserBenefits(ctx context.Context, userID types.ID, f benefit.Filter) ([]benefit.Benefit, int, error) {
	if err := f.Validate(); err != nil {
		return nil, 0, err
	}
	
    return j.benefits.ListBenefits(ctx, benefit.Filter{
        ClaimDateRange: f.ClaimDateRange,
        Claimed:        f.Claimed,
        AssistedID:     userID,
        Filter:         f.Filter,
    })
}