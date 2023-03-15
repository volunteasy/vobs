package distribution

import (
	"context"
    "govobs/core/types"
)

type Actions interface {
	CreateDistribution(ctx context.Context, d Distribution) error
	ListDistributions(ctx context.Context, f Filter) ([]Distribution, int, error)
	GetDistribution(ctx context.Context, id types.ID) (Distribution, error)
	UpdateDistribution(ctx context.Context, distribution Distribution) error
}
