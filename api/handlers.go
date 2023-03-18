package api

import (
	"context"
	mid "govobs/api/middleware"
	"govobs/api/request"
	"govobs/api/send"
	"govobs/obs"
	"net/http"
	"strconv"

	"github.com/go-chi/chi/v5"
	"github.com/go-chi/chi/v5/middleware"
	"github.com/newrelic/go-agent/v3/newrelic"
	"github.com/sirupsen/logrus"
)

type Deps struct {
	Logger *logrus.Entry
}

func Handler(deps Deps) http.Handler {
	router := chi.NewRouter()

	router.Use(
		middleware.RealIP,
		middleware.RequestID,
		mid.Logging(deps.Logger),
		middleware.Recoverer,
	)

	router.HandleFunc(
		newrelic.WrapHandleFunc(obs.Tracer(), "/",
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

				obs.Log(ctx).
					Infof("got message %s & num %d", p, i)

				return send.Data(200, map[string]string{
					"phrase": p,
				})
			}),
		),
	)

	return router
}
