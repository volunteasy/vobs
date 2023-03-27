package types

import (
	"fmt"
	"net/http"
)

type Error struct {
	Code    string `json:"code"`
	Message string `json:"message"`
	Status  int    `json:"-"`
}

func NewError(message, code string, status int) error {
	if code == "" {
		code = "internal_server_error"
	}

	if status == 0 {
		status = 500
	}

	return Error{
		Code:    code,
		Message: message,
		Status:  status,
	}
}

func ErrAlreadyExists(message string) Error {
	return Error{
		Code:    "err_already_exists",
		Message: message,
		Status:  http.StatusPreconditionFailed,
	}
}

func ErrMissingParentID(message string) Error {
	return Error{
		Code:    "err_missing_parent_id",
		Message: message,
		Status:  http.StatusPreconditionFailed,
	}
}

func ErrNotFound(message string) Error {
	return Error{
		Code:    "err_not_found",
		Message: message,
		Status:  http.StatusNotFound,
	}
}

func ErrInvalidField(message string) Error {
	return Error{
		Code:    "err_invalid_field",
		Message: message,
		Status:  http.StatusUnprocessableEntity,
	}
}

func (e Error) Error() string {
	return fmt.Sprintf("[%d] %s: %s", e.Status, e.Code, e.Message)
}
