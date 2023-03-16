package request

import (
	"context"
	"github.com/getsentry/sentry-go"
	"govobs/api/send"
	"net/http"
)

type Fn = func(ctx context.Context, r Request) send.Response

func Route(fn Fn) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		ctx := r.Context()

		res := fn(ctx, Request{Request: r})

		sentry.ConfigureScope(func(scope *sentry.Scope) {
			scope.SetUser(sentry.User{
				Email: "jane.doe@example.com",
			})
		})

		if err := send.Write(w, res); err != nil {
		}

		if err := res.Error; err != nil {
		}
	}
}
