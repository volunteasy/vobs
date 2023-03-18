package v1

import (
	"context"
	"govobs/api/request"
	"govobs/api/send"
	"govobs/core/user"
	userjobs "govobs/jobs/user"
	"net/http"
)

func CreateUser(users userjobs.Jobs) http.HandlerFunc {
	return request.Route(
		func(ctx context.Context, r request.Request) send.Response {

			var u user.User

			if err := r.JSONBody(&u); err != nil {
				return send.JSONError(err)
			}

			u, err := users.CreateUser(ctx, u)
			if err != nil {
				return send.FromError(err)
			}

			return send.CreatedWithID(u.ID)
		},
	)
}
