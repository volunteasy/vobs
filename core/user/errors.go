package user

import "errors"

var (
	ErrInvalidName = errors.New("name must have from 1 to 50 characters")
	ErrInvalidNickname = errors.New("nickname must have from 1 to 12 characters and be unique")
)