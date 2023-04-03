package users

import (
	"context"
	"net/http"

	"govobs/app/api/rest"
	"govobs/app/core/types"
)

// @Summary		Get user
// @Description	Gets an user by their unique ID
// @Tags			User
// @Param			userID	path	string	true	"The user ID. Same provided by cognito API"
//
// @Produce		json
//
// @Success		200	{object}	rest.Response{data=user.User{}}
// @Failure		400	{object}	rest.Response{error=types.Error{}} "In case the ID sent is invalid"
// @Failure		404	{object}	rest.Response{error=types.Error{}}
// @Failure		500	{object}	rest.Response{error=types.Error{}}
// @Router			/api/v1/users/{userID} [GET]
func getUser(users Users) http.HandlerFunc {
	return rest.Route(
		func(ctx context.Context, r rest.Request) rest.Response {
			id, err := r.ID(UserIDParam)
			if err != nil {
				return rest.Error(err)
			}

			u, err := users.GetUser(ctx, types.ID(id))
			if err != nil {
				return rest.Error(err)
			}

			return rest.Data(http.StatusOK, u)
		},
	)
}
