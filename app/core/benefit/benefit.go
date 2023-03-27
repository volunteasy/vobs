package benefit

import (
	"time"

	"govobs/app/core/types"
)

type (
	Benefit struct {
		ID             types.ID
		AssistedID     types.UserID
		DistributionID types.ID
		QueuePosID     types.ID
		ClaimedAt      time.Time
	}

	Filter struct {
		ClaimDateRange types.DateRange
		DistributionID types.ID
		AssistedID     types.UserID
		OrgID          types.ID

		types.Filter
	}
)

func (b Benefit) Validate() error {
	if b.AssistedID != types.ZeroUserID {
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
