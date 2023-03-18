package middleware

import (
	"github.com/go-chi/chi/v5/middleware"
	"github.com/sirupsen/logrus"
	"govobs/telemetry"
	"net/http"
)

type LoggerContextKey struct{}

func Logging() func(http.Handler) http.Handler {
	return func(handler http.Handler) http.Handler {
		return http.HandlerFunc(func(writer http.ResponseWriter, request *http.Request) {
			handler.ServeHTTP(writer, request.WithContext(
				telemetry.LoggerToContext(
					request.Context(),
					logrus.WithFields(logrus.Fields{
						"client_ip":  request.RemoteAddr,
						"request_id": request.Context().Value(middleware.RequestIDKey),
					}),
				),
			))
		})
	}
}
