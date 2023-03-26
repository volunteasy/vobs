package types

import (
	"errors"
)

type Phone string

var (
	ErrInvalidPhone = errors.New("must provide a valid phone")
)

func (p Phone) Validate() error {
	if p == "" {
		return ErrInvalidPhone
	}

	return nil
}
