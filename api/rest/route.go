package rest

import (
	"context"
	"encoding/json"
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

		code, header := res.code, w.Header()

		header.Set("Content-Type", "application/json")
		for _, h := range res.headers {
			header.Set(h.Key, h.Value)
		}

		w.WriteHeader(code)
		if res.Data != nil || res.Error != nil {
			err := json.NewEncoder(w).Encode(res)
			if err != nil {
				log.WithError(err).Error("failed writing response")
			}
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
