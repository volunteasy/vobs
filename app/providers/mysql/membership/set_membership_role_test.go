package membership

import (
	"context"
	"testing"

	"govobs/app/core/membership"
	"govobs/app/providers/mysql/conn/transaction"
	"govobs/app/providers/mysql/tests"

	"github.com/stretchr/testify/assert"
)

func TestSetMembershipRole(t *testing.T) {
	type args struct {
		m membership.Membership
	}

	for _, tt := range []struct {
		name    string
		args    args
		wantErr error
	}{
		{
			name: "should set role",
			args: args{
				m: membership.Membership{
					UserID: 1,
					OrgID:  1,
					Role:   membership.RoleAssisted,
				},
			},
		},
		{
			name: "should not set role for membership not found",
			args: args{
				m: membership.Membership{
					UserID: 10,
					OrgID:  1,
					Role:   membership.RoleAssisted,
				},
			},
			wantErr: membership.ErrNotFound,
		},
	} {
		t.Run(tt.name, func(t *testing.T) {
			ctx, closeTX := transaction.NewTransactionOpener(tests.NewDatabase(t))(context.Background())
			defer closeTX(ctx)

			err := actions{}.SetMembershipRole(ctx, tt.args.m)
			assert.ErrorIs(t, err, tt.wantErr)
		})
	}
}
