package distribution

import "errors"

var (
	ErrInvalidName = errors.New("name must have from 1 to 50 characters")

	ErrNoAddress = errors.New("a distribution must have an address where it is going happen")

	ErrNoDate = errors.New("cannot create a distribution with a past or empty date")

	ErrTooMuchAssisted = errors.New("the number of items has to be bigger or equal the number of benefits")
)