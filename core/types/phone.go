package types

import (
	"errors"
	"fmt"
)

type Phone struct {
	CountryCode string
	AreaCode    string
	PhoneNumber string
}

var (
	ErrInvalidPhone = errors.New("must provide a valid phone")
)

func (p Phone) Validate() error {
	if p.PhoneNumber == "" {
		return ErrInvalidPhone
	}

	return nil
}

// String returns a textual representation of this phone
func (p Phone) String() string {
	return fmt.Sprintf("%s %s %s", p.CountryCode, p.AreaCode, p.PhoneNumber)
}
