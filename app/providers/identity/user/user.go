package user

import (
	"github.com/aws/aws-sdk-go/service/cognitoidentityprovider"
	"govobs/app/core/user"
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
