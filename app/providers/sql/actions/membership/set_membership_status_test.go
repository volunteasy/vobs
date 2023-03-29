package membership

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"
	"govobs/app/core/membership"
	"govobs/app/providers/sql/transaction"
	"govobs/app/test/settings"
)

func TestSetMembershipStatus(t *testing.T) {
	type args struct {
		m membership.Membership
	}
	tests := []struct {
		name    string
		args    args
		wantErr error
	}{
		{
			name: "should set role",
			args: args{
				m: membership.Membership{
					UserID: "c5c0e5b5-0b5a-44ec-9d37-f43b9f16a072",
					OrgID:  1,
					Status: membership.StatusDeclined,
				},
			},
		},
		{
			name: "should not set role for membership not found",
			args: args{
				m: membership.Membership{
					UserID: "c5c0e5b5-0b5a-44ec-9d37-f43b9f16a075",
					OrgID:  1,
					Status: membership.StatusDeclined,
				},
			},
			wantErr: membership.ErrNotFound,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			ctx, closeTX := transaction.NewTransactionOpener(settings.DBTest(t))(context.Background())
			defer closeTX(ctx)

			err := actions{}.SetMembershipStatus(ctx, tt.args.m)
			assert.ErrorIs(t, err, tt.wantErr)
		})
	}
}
