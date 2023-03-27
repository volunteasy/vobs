package rest

import (
	"encoding/json"
	"errors"
	"fmt"
	"net/http"

	"govobs/app/core/types"
)

type Response struct {
	Error   interface{} `json:"error"`
	Data    interface{} `json:"data"`
	code    int
	headers []Header
}

type Header struct {
	Key, Value string
}

func Data(status int, data interface{}, headers ...Header) Response {
	return Response{
		Data:    data,
		code:    status,
		headers: headers,
	}
}

func Error(err error, headers ...Header) Response {
	// If error is of domain type, use it to build response
	var e types.Error
	if errors.As(err, &e) {
		return Response{
			Error:   e,
			code:    e.Status,
			headers: headers,
		}
	}

	var jsonErr *json.UnmarshalTypeError
	if errors.As(err, &jsonErr) {
		return Response{
			code:    http.StatusBadRequest,
			headers: headers,
			Error: types.Error{
				Code:    "invalid_request_body",
				Message: fmt.Sprintf("%s.%s: should not be of type %s", jsonErr.Struct, jsonErr.Field, jsonErr.Value),
			},
		}
	}

	// Return 500 if error is not from domain
	return Response{
		code:    http.StatusInternalServerError,
		headers: headers,
		Error: types.Error{
			Message: "Ops! An error occurred and we could not process your request",
			Code:    "internal_server_error",
		},
	}
}
