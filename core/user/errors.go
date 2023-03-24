package user

import (
	"govobs/core/types"
)

var (
	ErrInvalidName   = types.ErrInvalidField("O nome do usuário precisa ter entre 1 a 50 caracteres")
	ErrNoExternalID  = types.ErrMissingParentID("É preciso informar um ID único para este usuário")
	ErrNotFound      = types.ErrNotFound("Ops! Parece que este usuário não existe")
	ErrAlreadyExists = types.ErrAlreadyExists("Ops, esse documento já está sendo usado por outra conta")
)
