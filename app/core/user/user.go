package user

import (
	"govobs/app/core/membership"
	"govobs/app/core/types"
)

type (
	User struct {
		ID       types.ID       `json:"id"`
		Document types.Document `json:"document"`
		Phone    types.Phone    `json:"phone"`
		Name     string         `json:"name"`
	}

	Filter struct {
		Role           membership.Role
		Identification string

		types.Filter
	}

	Enrolled struct {
		User
		Role   membership.Role
		Status membership.Status
	}
)

func (f Filter) Validate() error {
	if err := f.Role.Validate(); err != nil {
		return err
	}

	if err := f.Filter.Validate(); err != nil {
		return err
	}

	return nil
}

func (u User) Validate() error {
	if len(u.Name) == 0 || len(u.Name) > 250 {
		return ErrInvalidName
	}

	if u.ID == types.ZeroID {
		return ErrNoExternalID
	}

	if err := u.Document.Validate(); err != nil {
		return err
	}

	if err := u.Phone.Validate(); err != nil {
		return err
	}

	return nil
}
