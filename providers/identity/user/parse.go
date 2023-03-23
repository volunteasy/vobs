package user

import (
	"context"
	"fmt"
	"govobs/core/types"
	"govobs/core/user"
	"govobs/obs"

	"github.com/aws/aws-sdk-go/service/cognitoidentityprovider"
)

var (
	subUserAttr      = "sub"
	nameUserAttr     = "name"
	phoneUserAttr    = "phone_number"
	documentUserAttr = "nickname"
)

var userAttributesSetters = map[string]func(u *user.User, val string) error{
	subUserAttr: func(u *user.User, val string) error {
		u.ID = types.UserID(val)
		return nil
	},

	nameUserAttr: func(u *user.User, val string) error {
		u.Name = val
		return nil
	},

	documentUserAttr: func(u *user.User, val string) error {
		u.Document = types.Document(val)
		return nil
	},

	phoneUserAttr: func(u *user.User, val string) error {
		u.Phone = types.Phone(val)
		return nil
	},
}

func userFromAttributes(ctx context.Context, attributes []*cognitoidentityprovider.AttributeType) (user.User, error) {
	u := user.User{}

	for _, attr := range attributes {
		if attr.Name == nil {
			continue
		}

		setter, ok := userAttributesSetters[*attr.Name]
		if !ok {
			continue
		}

		if attr.Value == nil {
			obs.Log(ctx).
				WithField("attribute", *attr.Name).
				Warn("attribute was empty or nil")

			return user.User{}, fmt.Errorf("attribute %s was empty or nil for user", *attr.Name)
		}

		setter(&u, *attr.Value)
	}

	if err := u.Validate(); err != nil {
		obs.Log(ctx).
			WithError(err).
			Warn("user retrieved from cognito was not valid")

		return user.User{}, fmt.Errorf("user retrieved from cognito was not valid")
	}

	return u, nil
}
