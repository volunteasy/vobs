package membership

import "errors"

var (
	ErrNoOrganizationID = errors.New("must have an organization id")
	ErrNoUserID = errors.New("must have an user ID")

	ErrInvalidRole = errors.New("must provide a valid role")

	ErrInvalidStatus = errors.New("must provide a valid status")
)