package benefit

import "errors"

var (
	ErrNoDistributionID = errors.New("must have a distribution id")
	ErrNoAssistedID = errors.New("must have an assisted ID")
)
