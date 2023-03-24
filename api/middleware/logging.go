package middleware

import (
	"govobs/obs"
	"net/http"

	"github.com/sirupsen/logrus"
)

type LoggerContextKey struct{}

func Logging(entry *logrus.Entry) func(http.Handler) http.Handler {
	return func(handler http.Handler) http.Handler {
		return http.HandlerFunc(func(writer http.ResponseWriter, request *http.Request) {
			handler.ServeHTTP(writer, request.WithContext(
				obs.LoggerToContext(
					request.Context(),
					entry.WithFields(logrus.Fields{
						"request_id": writer.Header().Get(requestIDHeader),
					}),
				),
			))
		})
	}
}
