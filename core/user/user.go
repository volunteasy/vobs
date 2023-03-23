package user

import (
	"govobs/core/membership"
	"govobs/core/types"
)

type (
	User struct {
		ID         types.ID
		ExternalID string
		Name       string
		Nickname   string
		Document   types.Document
		Phone      types.Phone
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

	if len(u.Nickname) == 0 || len(u.Nickname) > 12 {
		return ErrInvalidNickname
	}

	if u.ExternalID == "" {
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
