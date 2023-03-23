package request

import (
	"govobs/core/user"
	"net/http"
)

var InternalServerError = Error{
	Code:    "ERR_INTERNAL_SERVER_ERROR",
	Message: "Ops! An unexpected error happened here. Please try again later",
	status:  http.StatusInternalServerError,
}

var failures = map[error]Error{
	user.ErrAlreadyExists: {
		Code:    "ERR_USER_ALREADY_EXISTS",
		Message: "Ops! O documento informado já está cadastrado em outra conta",
		status:  http.StatusPreconditionFailed,
	},
}
