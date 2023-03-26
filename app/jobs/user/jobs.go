package user

import (
	"context"
	"govobs/app/core/benefit"
	"govobs/app/core/membership"
	"govobs/app/core/organization"
	"govobs/app/core/types"
	"govobs/app/core/user"
)

//go:generate moq -fmt goimports -out jobs_mock.go . Jobs:JobsMock

type (
	Jobs interface {
		// ValidateUser receives an user object and validates it according to business rules.
		// It also checks if the document already exists
		ValidateUser(ctx context.Context, u user.User) error

		// GetUser returns the user details sucn as address, phone, document, name and nickname
		GetUser(ctx context.Context, id types.UserID) (user.User, error)

		// ListUserBenefits shows the benefits this user has claimed or showed interest
		ListUserBenefits(ctx context.Context, userID types.UserID, f benefit.Filter) ([]benefit.Benefit, int, error)

		// EnrollOrganization adds a membership relation between an organization and an user. This membership must
		// be approved to take effect.
		EnrollOrganization(ctx context.Context, m membership.Membership) (membership.Membership, error)

		// ListUserEnrollments returns all the organizations a user has enrolled him or herself to, along with the enrollment status and role assigned
		ListUserEnrollments(ctx context.Context, userID types.UserID, f organization.Filter) ([]organization.Enrollment, int, error)
	}

	jobs struct {
		users         user.Actions
		benefits      benefit.Actions
		membership    membership.Actions
		organizations organization.Actions
		createID      types.IDCreator
	}
)

func NewJobs(
	users user.Actions,
	benefits benefit.Actions,
	membership membership.Actions,
	organizations organization.Actions,
	createID types.IDCreator,
) Jobs {
	return jobs{
		users,
		benefits,
		membership,
		organizations,
		createID,
	}
}
