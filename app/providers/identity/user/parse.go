package user

import (
	"context"
	"fmt"

	"github.com/aws/aws-sdk-go/service/cognitoidentityprovider"
	"govobs/app/core/types"
	"govobs/app/core/user"
	"govobs/app/obs"
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

var userAttributesGetters = map[string]func(u user.User, attr *cognitoidentityprovider.AttributeType) error{
	subUserAttr: func(u user.User, attr *cognitoidentityprovider.AttributeType) error {
		str := string(u.ID)
		attr.Value = &str
		return nil
	},

	nameUserAttr: func(u user.User, attr *cognitoidentityprovider.AttributeType) error {
		attr.Value = &u.Name
		return nil
	},

	documentUserAttr: func(u user.User, attr *cognitoidentityprovider.AttributeType) error {
		str := string(u.Document)
		attr.Value = &str
		return nil
	},

	phoneUserAttr: func(u user.User, attr *cognitoidentityprovider.AttributeType) error {
		str := string(u.Phone)
		attr.Value = &str
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

func userToAttributes(ctx context.Context, u user.User) ([]*cognitoidentityprovider.AttributeType, error) {
	attributes := make([]*cognitoidentityprovider.AttributeType, len(userAttributesGetters))

	idx := 0
	for key, fn := range userAttributesGetters {
		attr := &cognitoidentityprovider.AttributeType{
			Name: &key,
		}

		fn(u, attr)

		attributes[idx] = attr
		idx++
	}
	return attributes, nil
}
