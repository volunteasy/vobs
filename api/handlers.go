package api

import (
	"context"
	"github.com/go-chi/chi/v5"
	"github.com/go-chi/chi/v5/middleware"
	"github.com/newrelic/go-agent/v3/newrelic"
	mid "govobs/api/middleware"
	"govobs/api/request"
	"govobs/api/send"
	"govobs/telemetry"
	"net/http"
	"strconv"
)

func Handler() http.Handler {
	router := chi.NewRouter()

	router.Use(
		middleware.RealIP,
		middleware.RequestID,
		mid.Logging(),
		middleware.Recoverer,
	)

	router.HandleFunc(
		newrelic.WrapHandleFunc(telemetry.Tracer(), "/",
			request.Route(func(ctx context.Context, r request.Request) send.Response {
				p := r.URL.Query().Get("phrase")

				i, err := strconv.Atoi(r.URL.Query().Get("i"))
				if err != nil {
					return send.Status(400)
				}

				if i < 0 {
					return send.Status(400)
				}

				if i == 0 {
					panic("value must NOT be zero")
				}

				telemetry.LoggerFromContext(ctx).
					Infof("got message %s & num %d", p, i)

				return send.Data(200, map[string]string{
					"phrase": p,
				})
			}),
		),
	)

	return router
}
