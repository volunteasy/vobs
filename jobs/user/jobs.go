package user

import (
	"context"
	"govobs/core/benefit"
	"govobs/core/membership"
	"govobs/core/organization"
	"govobs/core/types"
	"govobs/core/user"
)

type (
	Jobs interface {
		// CreateUser is the first interaction a new client has with our service. It persists the user data in our DB
		CreateUser(ctx context.Context, u user.User) (user.User, error)

		// GetUser returns the user details sucn as address, phone, document, name and nickname
		GetUser(ctx context.Context, id types.ID) (user.User, error)

		// ListUserBenefits shows the benefits this user has claimed or showed interest
		ListUserBenefits(ctx context.Context, userID types.ID, f benefit.Filter) ([]benefit.Benefit, int, error)

		// EnrollOrganization adds a membership relation between an organization and an user. This membership must
		// be approved to take effect.
		EnrollOrganization(ctx context.Context, m membership.Membership) (membership.Membership, error)

		// ListUserEnrollments returns all the organizations a user has enrolled him or herself to, along with the enrollment status and role assigned
		ListUserEnrollments(ctx context.Context, userID types.ID, f organization.Filter) ([]organization.Enrollment, int, error)
	}

	PhoneVerifier interface {
		// VerifyPhone sends a confirmation number to the given phone number
		VerifyPhone(ctx context.Context, phone types.Phone) error

		// CheckVerificationCode checks if the confirmation code informed is the one previously sent to the user's phone number
		CheckVerificationCode(ctx context.Context, code string) error
	}

	jobs struct {
		users         user.Actions
		benefits      benefit.Actions
		membership    membership.Actions
		organizations organization.Actions
		phoneauth     PhoneVerifier
		createID      types.IDCreator
	}
)

func NewJobs(
	users user.Actions,
	benefits benefit.Actions,
	membership membership.Actions,
	organizations organization.Actions,
	phoneauth PhoneVerifier,
	createID types.IDCreator,
) Jobs {
	return jobs{
		users,
		benefits,
		membership,
		organizations,
		phoneauth,
		createID,
	}
}
