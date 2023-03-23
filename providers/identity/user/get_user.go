package user

import (
	"context"
	"govobs/core/types"
	"govobs/core/user"
	"govobs/obs"

	"github.com/aws/aws-sdk-go/aws/awserr"
	"github.com/aws/aws-sdk-go/service/cognitoidentityprovider"
)

func (a actions) GetUser(ctx context.Context, id types.UserID) (user.User, error) {
	out, err := a.identity.AdminGetUserWithContext(ctx, (&cognitoidentityprovider.AdminGetUserInput{}).
		SetUserPoolId(a.poolID).
		SetUsername(string(id)),
	)

	if err != nil {
		if awsErr, ok := err.(awserr.Error); ok && awsErr.Code() == cognitoidentityprovider.ErrCodeUserNotFoundException {
			return user.User{}, user.ErrNotFound
		}

		obs.Log(ctx).
			WithError(err).
			Warn("could not retrieve user data from cognito")

		return user.User{}, err
	}

	u, err := userFromAttributes(ctx, out.UserAttributes)
	if err != nil {
		return user.User{}, err
	}

	return u, nil

}
