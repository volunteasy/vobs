package membership

import (
	"context"
	"testing"

	"govobs/app/core/membership"
	"govobs/app/providers/mysql/conn/transaction"
	"govobs/app/providers/mysql/tests"

	"github.com/stretchr/testify/assert"
)

func TestCreateMembership(t *testing.T) {
	t.Parallel()

	type args struct {
		m membership.Membership
	}

	for _, tt := range []struct {
		name    string
		args    args
		wantErr error
	}{
		{
			name: "should create membership successfully",
			args: args{
				m: membership.Membership{
					UserID: "1",
					OrgID:  1,
					Status: membership.StatusAccepted,
					Role:   membership.RoleAssisted,
				},
			},
		},
		{
			name: "should fail because membership already exists",
			args: args{
				m: membership.Membership{
					UserID: "c5c0e5b5-0b5a-44ec-9d37-f43b9f16a072",
					OrgID:  1,
					Status: membership.StatusAccepted,
					Role:   membership.RoleAssisted,
				},
			},
			wantErr: membership.ErrAlreadyExists,
		},
	} {
		tt := tt
		t.Run(tt.name, func(t *testing.T) {
			t.Parallel()

			ctx, closeTX := transaction.NewTransactionOpener(tests.NewDatabase(t))(context.Background())
			defer closeTX(ctx)

			err := actions{}.CreateMembership(ctx, tt.args.m)

			assert.ErrorIs(t, err, tt.wantErr)
		})
	}
}
