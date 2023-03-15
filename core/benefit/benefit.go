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
		ClaimDateRange types.DateRange
		DistributionID types.ID
		Claimed        bool
		AssistedID     types.ID
		OrgID          types.ID

		types.Filter
	}
)

func (b Benefit) Validate() error {
	if b.AssistedID != types.ZeroID {
		return ErrNoAssistedID
	}

	if b.DistributionID != types.ZeroID {
		return ErrNoDistributionID
	}

	return nil
}

func (b Benefit) Claimed() bool {
	return !b.ClaimedAt.IsZero()
}


func (f Filter) Validate() error {
	if err := f.ClaimDateRange.Validate(); err != nil {
		return err
	}

	if err := f.Filter.Validate(); err != nil {
		return err
	}

	return nil
}
