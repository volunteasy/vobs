package types

import (
    "errors"
    "time"
)

var (
	ErrInvalidDateRange = errors.New("end date must be equal or after start date")
	ErrInvalidListRange = errors.New("invalid range for this list")
)

type (
	Filter struct {
		Range ListRange
	}

	DateRange struct {
		Start time.Time
		End time.Time
	}

	ListRange struct {
		Start, End int
	}
)

func (f Filter) Validate() error {
	return f.Range.Validate()
}


func (d DateRange) Validate() error {
	if d.End.Before(d.Start) {
		return ErrInvalidDateRange
	}

	return nil
}

func (r ListRange) Validate() error {
	if !(r.Start >= 0 && r.Start <= r.End) {
		return ErrInvalidListRange
	}

	return nil
}

func (r ListRange) Limit() int {
	return r.End - r.Start + 1
}


func (r ListRange) Page() int {
	return (r.Start / r.Limit()) + 1
}