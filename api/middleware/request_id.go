package middleware

import (
	"context"
	"govobs/core/types"
	"net/http"
)

type requestIDContextKey struct{}

const requestIDHeader = "x-request-id"

func RequestID(idgen types.IDCreator) func(http.Handler) http.Handler {
	return func(handler http.Handler) http.Handler {
		return http.HandlerFunc(func(writer http.ResponseWriter, request *http.Request) {
			id := idgen().String()

			writer.Header().Set(requestIDHeader, id)
			handler.ServeHTTP(writer, request.WithContext(
				context.WithValue(request.Context(), requestIDContextKey{}, id),
			))
		})
	}
}
