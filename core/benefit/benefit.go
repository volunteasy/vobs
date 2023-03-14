package benefit

import (
	"govobs/core/types"
	"time"
)

type (
	Benefit struct {
		ID             types.ID
		AssistedID     types.ID
		DistributionID types.ID
		ClaimedAt      time.Time
	}

	Filter struct {
		ClaimStartDate time.Time
		ClaimEndDate   time.Time
		DistributionID types.ID
		Claimed        bool
		AssistedID     types.ID
		OrgID          types.ID

		types.Filter
	}
)

func (b Benefit) Validate() error {

	return nil
}

func (b Benefit) Claimed() bool {
	return !b.ClaimedAt.IsZero()
}
