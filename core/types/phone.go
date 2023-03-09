package types

import "fmt"

type Phone struct {
	CountryCode string
	AreaCode    string
	PhoneNumber string
}

func (p Phone) Validate() error {
	return nil
}

// String returns a textual representation of this phone
func (p Phone) String() string {
	return fmt.Sprintf("%s %s %s", p.CountryCode, p.AreaCode, p.PhoneNumber)
}
