package organization

import (
	"errors"
	"govobs/app/core/types"
)

var (
	ErrInvalidName = errors.New("name must have from 1 to 50 characters")

	ErrAlreadyExists = types.ErrAlreadyExists("Já existe uma organização cadastrada com esse documento")
)
