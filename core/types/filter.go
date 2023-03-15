package types

import (
    "errors"
    "time"
)

var (
	ErrInvalidDateRange = errors.New("end date must be equal or after start date")
)

type Filter struct {
}


type DateRange struct {
	Start time.Time
	End time.Time
}

func (f Filter) Validate() error {
	return nil
}


func (d DateRange) Validate() error {
	if d.End.Before(d.Start) {
		return ErrInvalidDateRange
	}

	return nil
}