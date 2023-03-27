package organization

import (
	"context"

	"govobs/app/core/types"
)

//go:generate moq -fmt goimports -out actions_mock.go . Actions:ActionsMock

type Actions interface {
	CreateOrganization(ctx context.Context, o Organization) error
	GetOrganization(ctx context.Context, id types.ID) (Organization, error)
	ListOrganizations(ctx context.Context, f Filter) ([]Organization, int, error)
	ListEnrollments(ctx context.Context, userID types.UserID, f Filter) ([]Enrollment, int, error)
}
