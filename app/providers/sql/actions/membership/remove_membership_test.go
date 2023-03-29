package membership

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"
	"govobs/app/core/membership"
	"govobs/app/core/types"
	"govobs/app/providers/sql/transaction"
	"govobs/app/test/settings"
)

func TestRemoveMembership(t *testing.T) {
	type args struct {
		userID         types.UserID
		organizationID types.ID
	}
	tests := []struct {
		name    string
		args    args
		wantErr error
	}{
		{
			name: "should remove membership",
			args: args{
				userID:         "c5c0e5b5-0b5a-44ec-9d37-f43b9f16a072",
				organizationID: 1,
			},
		},
		{
			name: "should fail membership deletion because it does not exist",
			args: args{
				userID:         "c5c0e5b5-0b5a-44ec-9d37-f43b9f16a072",
				organizationID: 88,
			},
			wantErr: membership.ErrNotFound,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			ctx, closeTX := transaction.NewTransactionOpener(settings.DBTest(t))(context.Background())
			defer closeTX(ctx)

			err := actions{}.RemoveMembership(ctx, tt.args.userID, tt.args.organizationID)

			assert.ErrorIs(t, err, tt.wantErr)
		})
	}
}
