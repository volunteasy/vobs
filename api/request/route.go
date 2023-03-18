package request

import (
	"context"
	"govobs/api/send"
	"govobs/obs"
	"net/http"
	"time"

	"github.com/newrelic/go-agent/v3/newrelic"
	"github.com/sirupsen/logrus"
)

type (
	Fn = func(ctx context.Context, r Request) send.Response
)

func Route(fn Fn) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		ctx := r.Context()
		now := time.Now().UTC()
		log := obs.Log(ctx)
		txn := newrelic.FromContext(ctx)

		res := fn(ctx, Request{
			Txn:     txn,
			Request: r,
		})

		if err := send.Write(w, res); err != nil {
			log.WithError(err).Error("failed writing response")
			txn.NoticeError(err)
		}

		if err := res.Error; err != nil {
			log = log.WithError(err)

			if res.Code >= http.StatusInternalServerError {
				txn.NoticeError(err)
			} else {
				txn.NoticeExpectedError(err)
			}
		}

		log.
			WithFields(logrus.Fields{
				"method": r.Method,
				"path":   r.URL.Path,
				"took":   time.Since(now),
				"status": res.Code,
			}).Log(logrus.InfoLevel)
	}
}
