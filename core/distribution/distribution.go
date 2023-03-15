package distribution

import (
	"govobs/core/types"
	"reflect"
	"time"
)

type (
	Distribution struct {
		ID                types.ID
		Name              string
		Description       string
		ContactInfo       types.Contact
		Date             time.Time
		Items           int
		BenefitsAllowed int
	}

	Filter struct {
		Search    string
		DateRange types.DateRange

		types.Filter
	}
)

func (d Distribution) Validate() error {
	if len(d.Name) == 0 || len(d.Name) > 50 {
		return ErrInvalidName
	}

	if reflect.DeepEqual(d.ContactInfo.Address, types.Address{}) {
		return ErrNoAddress
	}

	if d.Date.IsZero() {
		return ErrNoDate
	}

	if d.BenefitsAllowed > d.Items {
		return ErrTooMuchAssisted
	}

	return nil
}

func (f Filter) Validate() error {
	if err := f.DateRange.Validate(); err != nil {
		return err
	}

	if err := f.Filter.Validate(); err != nil {
		return err
	}

	return nil
}
