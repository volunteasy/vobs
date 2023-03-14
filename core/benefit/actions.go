package benefit

import (
	"context"
	"govobs/core/types"
	"time"
)

type Actions interface {
	Create(ctx context.Context, c Benefit) error
	List(ctx context.Context, f Filter) ([]Benefit, int, error)
	SetClaimedAt(ctx context.Context, id types.ID, at time.Time) error
	SetCollectionDate(ctx context.Context, id types.ID, date time.Time) error
}
