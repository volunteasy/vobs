package benefit

import (
	"context"
	"govobs/core/types"
	"time"
)

type Actions interface {
	CreateBenefit(ctx context.Context, c Benefit) error
	ListBenefits(ctx context.Context, f Filter) ([]Benefit, int, error)
	SetBenefitClaimDate(ctx context.Context, id types.ID, at time.Time) error
}
