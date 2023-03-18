package v1

import (
	"context"
	"govobs/api/request"
	"govobs/api/send"
	userjobs "govobs/jobs/user"
	"net/http"
)

func GetUser(users userjobs.Jobs) http.HandlerFunc {
	return request.Route(
		func(ctx context.Context, r request.Request) send.Response {

			id, err := r.ID(UserIDParam)
			if err != nil {
				return send.IDError(err)
			}

			u, err := users.GetUser(ctx, id)
			if err != nil {
				return send.FromError(err)
			}

			return send.Data(http.StatusOK, u)
		},
	)
}
