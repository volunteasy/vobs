package user

import (
	"context"
	"gotest.tools/v3/assert"
	"govobs/core/membership"
	"testing"
)

func TestJobs_EnrollOrganization(t *testing.T) {
	t.Parallel()

	type (
		args struct {
			ctx        context.Context
			membership membership.Membership
		}

		fields struct {
			memberships *membership.ActionsMock
		}

		test struct {
			name    string
			args    args
			fields  fields
			wantErr error
			want    membership.Membership
		}
	)

	for _, tc := range []test{
		{
			name: "should enroll assisted membership successfully",
			args: args{
				ctx: context.Background(),
				membership: membership.Membership{
					UserID: 1,
					OrgID:  2,
					Role:   membership.RoleAssisted,
				},
			},
			fields: fields{
				memberships: &membership.ActionsMock{
					CreateMembershipFunc: func(ctx context.Context, m membership.Membership) error {
						return nil
					},
				},
			},
			wantErr: nil,
			want: membership.Membership{
				UserID: 1,
				OrgID:  2,
				Role:   membership.RoleAssisted,
				Status: membership.StatusAccepted,
			},
		},
		{
			name: "should enroll volunteer membership successfully",
			args: args{
				ctx: context.Background(),
				membership: membership.Membership{
					UserID: 1,
					OrgID:  2,
					Role:   membership.RoleVolunteer,
				},
			},
			fields: fields{
				memberships: &membership.ActionsMock{
					CreateMembershipFunc: func(ctx context.Context, m membership.Membership) error {
						return nil
					},
				},
			},
			wantErr: nil,
			want: membership.Membership{
				UserID: 1,
				OrgID:  2,
				Role:   membership.RoleVolunteer,
				Status: membership.StatusPending,
			},
		},
		{
			name: "should fail persistence of membership enrollment",
			args: args{
				ctx: context.Background(),
				membership: membership.Membership{
					UserID: 1,
					OrgID:  2,
					Role:   membership.RoleVolunteer,
				},
			},
			fields: fields{
				memberships: &membership.ActionsMock{
					CreateMembershipFunc: func(ctx context.Context, m membership.Membership) error {
						return membership.ErrInvalidRole
					},
				},
			},
			wantErr: membership.ErrInvalidRole,
			want:    membership.Membership{},
		},
		{
			name: "should fail with invalid role",
			args: args{
				ctx: context.Background(),
				membership: membership.Membership{
					UserID: 1,
					OrgID:  2,
					Role:   "chilling",
				},
			},
			wantErr: membership.ErrInvalidRole,
			want:    membership.Membership{},
		},
		{
			name: "should fail with invalid user ID",
			args: args{
				ctx: context.Background(),
				membership: membership.Membership{
					OrgID: 2,
					Role:  membership.RoleAssisted,
				},
			},
			wantErr: membership.ErrNoUserID,
			want:    membership.Membership{},
		},
		{
			name: "should fail with invalid organization ID",
			args: args{
				ctx: context.Background(),
				membership: membership.Membership{
					UserID: 1,
					Role:   membership.RoleAssisted,
				},
			},
			wantErr: membership.ErrNoOrganizationID,
			want:    membership.Membership{},
		},
	} {
		tc := tc

		t.Run(tc.name, func(t *testing.T) {
			t.Parallel()

			m, err := jobs{
				membership: tc.fields.memberships,
			}.EnrollOrganization(tc.args.ctx, tc.args.membership)

			assert.ErrorIs(t, err, tc.wantErr)
			assert.Equal(t, tc.want, m)
		})
	}
}
