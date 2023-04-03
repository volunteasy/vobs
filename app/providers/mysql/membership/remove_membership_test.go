package membership

import (
	"context"
	"testing"

	"govobs/app/core/membership"
	"govobs/app/core/types"
	"govobs/app/providers/mysql/conn/transaction"
	"govobs/app/providers/mysql/tests"

	"github.com/stretchr/testify/assert"
)

func TestRemoveMembership(t *testing.T) {
	type args struct {
		userID         types.ID
		organizationID types.ID
	}

	for _, tt := range []struct {
		name    string
		args    args
		wantErr error
	}{
		{
			name: "should remove membership",
			args: args{
				userID:         1,
				organizationID: 1,
			},
		},
		{
			name: "should fail membership deletion because it does not exist",
			args: args{
				userID:         89,
				organizationID: 88,
			},
			wantErr: membership.ErrNotFound,
		},
	} {
		t.Run(tt.name, func(t *testing.T) {
			ctx, closeTX := transaction.NewTransactionOpener(tests.NewDatabase(t))(context.Background())
			defer closeTX(ctx)

			err := actions{}.RemoveMembership(ctx, tt.args.userID, tt.args.organizationID)

			assert.ErrorIs(t, err, tt.wantErr)
		})
	}
}
