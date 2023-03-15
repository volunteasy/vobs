package organization

import "errors"

var (
	ErrInvalidName = errors.New("name must have from 1 to 50 characters")
)