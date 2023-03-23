package request

import (
	"context"
	"govobs/obs"
	"net/http"
	"time"

	"github.com/sirupsen/logrus"
)

type (
	Fn = func(ctx context.Context, r Request) Response
)

func Route(fn Fn) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		ctx := r.Context()
		now := time.Now().UTC()
		log := obs.Log(ctx)

		res := fn(ctx, Request{
			Request: r,
		})

		if err := Write(w, res); err != nil {
			log.WithError(err).Error("failed writing response")
		}

		if err := res.err; err != nil {
			log = log.WithError(err)
		}

		log.
			WithFields(logrus.Fields{
				"method": r.Method,
				"path":   r.URL.Path,
				"took":   time.Since(now),
				"status": res.code,
			}).Log(logrus.InfoLevel)
	}
}
