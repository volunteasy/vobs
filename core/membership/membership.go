package membership

import "govobs/core/types"

const (
	RoleNone        = ""
	RoleVolunteer   = "volunteer"
	RoleOwner       = "owner"
	RoleBeneficiary = "beneficiary"
)

const (
	StatusNone     = ""
	StatusAccepted = "accepted"
	StatusPending  = "pending"
	StatusDeclined = "declined"
)

type (
	Membership struct {
		UserID types.ID
		OrgID  types.ID
		Status Status
		Role   Role
	}

	Role string

	Status string
)
