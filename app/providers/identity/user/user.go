package user

import (
	"context"

	"govobs/app/core/user"

	"github.com/aws/aws-sdk-go/service/cognitoidentityprovider"
)

type actions struct {
	poolID, clientID string
	identity         *cognitoidentityprovider.CognitoIdentityProvider
}

func NewActions(userPoolID, clientID string, cip *cognitoidentityprovider.CognitoIdentityProvider) user.Actions {
	return actions{
		poolID:   userPoolID,
		clientID: clientID,
		identity: cip,
	}
}

func (a actions) CreateUser(ctx context.Context, u user.User) error {
	attr, err := userToAttributes(ctx, u)
	if err != nil {
		return err
	}

	userid := string(rune(u.ID))

	_, err = a.identity.AdminCreateUser(&cognitoidentityprovider.AdminCreateUserInput{
		UserAttributes: attr,
		UserPoolId:     &a.poolID,
		Username:       &userid,
	})

	if err != nil {
		return err
	}

	return nil
}
