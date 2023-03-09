package types

import "fmt"

type Address struct {
	ZipCode, HouseNumber, StreetName, Complement, District, City, State, Country string
}

func (a Address) Validate() error {
	return nil
}

// String returns a textual representation of this address, following the standard defined by Correios
func (a Address) String() string {
	return fmt.Sprintf("%s, Nº %s, %s - %s. CEP: %s. %s, %s - %s", a.StreetName, a.HouseNumber, a.Complement, a.District, a.ZipCode, a.City, a.State, a.Country)
}
