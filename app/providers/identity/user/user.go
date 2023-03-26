package user

import (
	"govobs/app/core/user"

	"github.com/aws/aws-sdk-go/service/cognitoidentityprovider"
)

type actions struct {
	poolID   string
	identity *cognitoidentityprovider.CognitoIdentityProvider
}

func NewActions(userPoolID string, cip *cognitoidentityprovider.CognitoIdentityProvider) user.Actions {
	return actions{
		poolID:   userPoolID,
		identity: cip,
	}
}
