package oeganization

import (
	"context"
    "govobs/core/benefit"
    "govobs/core/distribution"
    "govobs/core/membership"
    "govobs/core/organization"
    "govobs/core/types"
	"govobs/core/user"
)

type (
	Jobs interface {
		CreateUser(ctx context.Context, u user.User) (user.User, error)
		GetUser(ctx context.Context, id types.ID) (user.User, error)
		ListUserBenefits(ctx context.Context, userID types.ID, f benefit.Filter) ([]benefit.Benefit, int, error)
	}


	jobs struct {
		users user.Actions
		benefits benefit.Actions
		memberships membership.Actions
		distributions distribution.Actions
		organizations organization.Actions
		createID types.IDCreator
	}
)

func NewJobs(
	users user.Actions,
	benefits benefit.Actions,
	memberships membership.Actions,
	distributions distribution.Actions,
	organizations organization.Actions,
	createID types.IDCreator,
) Jobs {
	return jobs{
		users,
		benefits,
		memberships,
		distributions,
		organizations,
		createID,
	}
}