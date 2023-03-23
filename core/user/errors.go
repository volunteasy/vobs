package user

import "errors"

var (
	ErrInvalidName   = errors.New("name must have from 1 to 50 characters")
	ErrNoExternalID  = errors.New("must provide an external id")
	ErrNotFound      = errors.New("user not found or does not exist")
	ErrAlreadyExists = errors.New("user already exists")
)
