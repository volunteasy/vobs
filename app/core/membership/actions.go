package membership

import (
	"context"
	"govobs/app/core/types"
)

//go:generate moq -fmt goimports -out actions_mock.go . Actions:ActionsMock

type Actions interface {
	CreateMembership(ctx context.Context, m Membership) error
	RemoveMembership(ctx context.Context, userID types.UserID, organizationID types.ID) error
	SetMembershipStatus(ctx context.Context, m Membership) error
	SetMembershipRole(ctx context.Context, m Membership) error
}
