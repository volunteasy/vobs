package types

import (
	"errors"
	"fmt"
	"regexp"
)

type Address struct {
	ZipCode     string `json:"zipCode,omitempty"`
	HouseNumber string `json:"houseNumber,omitempty"`
	StreetName  string `json:"streetName,omitempty"`
	Complement  string `json:"complement,omitempty"`
	District    string `json:"district,omitempty"`
	City        string `json:"city,omitempty"`
	State       string `json:"state,omitempty"`
	Country     string `json:"country,omitempty"`
}

var ErrInvalidAddress = errors.New("must provide a valid address")

var zipCodeRegexp = regexp.MustCompile(`^\d{5}-?\d{3}$`)

func (a Address) Validate() error {
	if a.ZipCode == "" ||
		!zipCodeRegexp.MatchString(a.ZipCode) ||
		a.HouseNumber == "" ||
		a.StreetName == "" ||
		a.District == "" ||
		a.City == "" ||
		a.State == "" ||
		a.Country == "" {
		return ErrInvalidAddress
	}

	return nil
}

// String returns a textual representation of this address, following the standard defined by Correios
func (a Address) String() string {
	return fmt.Sprintf("%s, Nº %s, %s - %s. CEP: %s. %s, %s - %s", a.StreetName, a.HouseNumber, a.Complement, a.District, a.ZipCode, a.City, a.State, a.Country)
}
