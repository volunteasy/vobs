package membership

import "govobs/core/types"

const (
	RoleNone      = ""
	RoleVolunteer = "volunteer"
	RoleOwner     = "owner"
	RoleAssisted  = "assisted"
)

const (
	StatusNone     = ""
	StatusAccepted = "accepted"
	StatusPending  = "pending"
	StatusDeclined = "declined"
)

type (
	Membership struct {
		UserID types.UserID
		OrgID  types.ID
		Status Status
		Role   Role
	}

	Role string

	Status string
)

func (r Role) Validate() error {
	switch r {
	case RoleAssisted, RoleVolunteer, RoleOwner:
		return nil
	}

	return ErrInvalidRole
}

func (s Status) Validate() error {
	switch s {
	case StatusAccepted, StatusDeclined, StatusPending:
		return nil
	}

	return ErrInvalidStatus
}

func (m Membership) Validate() error {
	if m.OrgID == types.ZeroID {
		return ErrNoOrganizationID
	}

	if m.UserID == types.ZeroUserID {
		return ErrNoUserID
	}

	if err := m.Role.Validate(); err != nil {
		return err
	}

	if err := m.Status.Validate(); err != nil {
		return err
	}

	return nil
}
