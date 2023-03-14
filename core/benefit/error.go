package benefit

import "errors"

var (
	ErrInvalidCollectionDate = errors.New("must provide a date in the future")
)
