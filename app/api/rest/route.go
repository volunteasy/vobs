package rest

import (
	"context"
	"encoding/json"
	"fmt"
	"govobs/app/obs"
	"net/http"
	"runtime/debug"
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

		var res Response
		func() {
			res = fn(ctx, Request{
				Request: r,
			})

			defer func() {
				if rvr := recover(); rvr != nil {
					if rvr == http.ErrAbortHandler {
						panic(rvr)
					}

					log.
						WithError(fmt.Errorf("%s", debug.Stack())).
						WithField("path", r.URL.Path).
						Error("panic serving route")

					res = Error(fmt.Errorf("panic serving function"))
				}
			}()
		}()

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
