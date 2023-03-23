package v1

import (
	"context"
	"govobs/api/request"
	"govobs/core/user"
	userjobs "govobs/jobs/user"
	"net/http"
)

//	@Summary		Validate user input
//	@Description	Checks if all the inputs creates a valid user. Also validates if there is already an account with the given document
//	@Tags			User
//	@Param			user	body	user.User	true	"User data to be validated"
//
//	@Accept			json
//	@Produce		json
//
//	@Success		204
//	@Failure		412	{object}	request.Response{error=request.Error{}}
//	@Failure		422	{object}	request.Response{error=request.Error{}}
//	@Failure		500	{object}	request.Response{error=request.Error{}}
//	@Router			/api/v1/users [POST]
func ValidateUser(users userjobs.Jobs) http.HandlerFunc {
	return request.Route(
		func(ctx context.Context, r request.Request) request.Response {

			var u user.User

			if err := r.JSONBody(&u); err != nil {
				return request.SendJSONError(err)
			}

			err := users.ValidateUser(ctx, u)
			if err != nil {
				return request.SendFromError(err)
			}

			return request.SendData(nil, request.WithStatus(http.StatusNoContent))
		},
	)
}
