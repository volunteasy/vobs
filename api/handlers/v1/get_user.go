package v1

import (
	"context"
	"govobs/api/request"
	"govobs/core/types"
	userjobs "govobs/jobs/user"
	"net/http"
)

func GetUser(users userjobs.Jobs) http.HandlerFunc {
	return request.Route(
		func(ctx context.Context, r request.Request) request.Response {
			id := r.Param(UserIDParam)

			u, err := users.GetUser(ctx, types.UserID(id))
			if err != nil {
				return request.SendFromError(err)
			}

			return request.SendData(u)
		},
	)
}
