package benefit

import (
	"context"
	"time"

	"govobs/app/core/types"
)

//go:generate moq -fmt goimports -out actions_mock.go . Actions:ActionsMock

type Actions interface {
	CreateBenefit(ctx context.Context, c Benefit) error
	ListBenefits(ctx context.Context, f Filter) ([]Benefit, int, error)
	SetBenefitClaimDate(ctx context.Context, id types.ID, at time.Time) error
}
