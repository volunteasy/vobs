package user

import (
	"context"
	"fmt"

	"github.com/aws/aws-sdk-go/service/cognitoidentityprovider"
	"govobs/app/core/types"
	"govobs/app/core/user"
	"govobs/app/obs"
)

func (a actions) GetUserWithDocument(ctx context.Context, document types.Document) (user.User, error) {
	list, err := a.identity.ListUsersWithContext(ctx, (&cognitoidentityprovider.ListUsersInput{}).
		SetUserPoolId(a.poolID).
		SetFilter(fmt.Sprintf(`username="%s"`, document)),
	)
	if err != nil {
		obs.Log(ctx).
			WithError(err).
			Warn("could not retrieve user data from cognito")

		return user.User{}, err
	}

	if len(list.Users) == 0 {
		return user.User{}, user.ErrNotFound
	}

	out := list.Users[0]

	u, err := userFromAttributes(ctx, out.Attributes)
	if err != nil {
		return user.User{}, err
	}

	return u, nil
}
