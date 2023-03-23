package request

import (
	"encoding/json"
	"errors"
	"fmt"
	"net/http"
)

func SendData(data interface{}, options ...ResponseOptions) Response {
	return build(Response{Data: data}, options...)
}

func sendError(data Error, err error, options ...ResponseOptions) Response {
	return build(Response{Error: data}, append(options, WithError(err), WithStatus(data.status))...)
}

func SendCreated(location string) Response {
	return SendData(
		WithStatus(http.StatusCreated),
		WithHeader("Location", location),
	)
}

func SendJSONError(err error, options ...ResponseOptions) Response {
	var jsonErr *json.UnmarshalTypeError
	var target string

	if errors.As(err, &jsonErr) {
		target = fmt.Sprintf("%s.%s: should not be of type %s", jsonErr.Struct, jsonErr.Field, jsonErr.Value)
	}

	return sendError(Error{
		Code:    "INVALID_REQUEST_BODY",
		Message: "The request body could not be parsed. Please verify if it is not offending the endpoint contract",
		status:  http.StatusBadRequest,
		Target:  target,
	}, err, options...)
}

func SendIDError(err error, options ...ResponseOptions) Response {
	return sendError(Error{
		Code:    "INVALID_ID",
		Message: "The id provided is invalid",
		status:  http.StatusBadRequest,
	}, err, options...)
}

func SendFromError(err error, options ...ResponseOptions) Response {
	failure, ok := failures[err]
	if !ok {
		failure = InternalServerError
		for mappedError, mappedFailure := range failures {
			if errors.Is(err, mappedError) {
				failure = mappedFailure
				break
			}
		}
	}

	return sendError(failure, err, options...)
}

func build(r Response, options ...ResponseOptions) Response {
	if len(options) == 0 {
		return r
	}

	return build(options[0](r), options[1:]...)
}
