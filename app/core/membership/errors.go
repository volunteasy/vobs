package membership

import (
	"errors"

	"govobs/app/core/types"
)

var (
	ErrNoOrganizationID = errors.New("must have an organization id")
	ErrNoUserID         = errors.New("must have an user ID")

	ErrInvalidRole = errors.New("must provide a valid role")

	ErrInvalidStatus = errors.New("must provide a valid status")

	ErrAlreadyExists = types.ErrAlreadyExists("Não foi possível se inscrever. Você já está cadastrado nesta organização")

	ErrNotFound = types.ErrNotFound("Não encontramos essa inscrição nesta organização")
)
