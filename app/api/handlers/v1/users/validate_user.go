package users

import (
	"context"
	"govobs/app/api/rest"
	"govobs/app/core/user"
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
//	@Failure		412	{object}	rest.Response{error=types.Error{}}
//	@Failure		422	{object}	rest.Response{error=types.Error{}}
//	@Failure		500	{object}	rest.Response{error=types.Error{}}
//	@Router			/api/v1/users [POST]
func validateUser(users Users) http.HandlerFunc {
	return rest.Route(
		func(ctx context.Context, r rest.Request) rest.Response {

			var u user.User

			if err := r.JSONBody(&u); err != nil {
				return rest.Error(err)
			}

			err := users.ValidateUser(ctx, u)
			if err != nil {
				return rest.Error(err)
			}

			return rest.Data(http.StatusNoContent, nil)
		},
	)
}
