package user

import (
	"context"
	"govobs/core/types"
	"govobs/core/user"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestJobs_ValidateUser(t *testing.T) {
	t.Parallel()

	type (
		args struct {
			ctx  context.Context
			user user.User
		}

		fields struct {
			users *user.ActionsMock
		}

		test struct {
			name    string
			args    args
			fields  fields
			wantErr error
		}
	)

	for _, tc := range []test{
		{
			name: "should validate successfully",
			args: args{
				ctx: context.Background(),
				user: user.User{
					Name:     "Karim Andersson",
					Document: "50215322202",
					ID:       "FakeID",
					Phone:    "44332222",
				},
			},
			fields: fields{
				users: &user.ActionsMock{
					GetUserWithDocumentFunc: func(ctx context.Context, doc types.Document) (user.User, error) {
						return user.User{}, user.ErrNotFound
					},
				},
			},
			wantErr: nil,
		},
		{
			name: "should fail because document already exists",
			args: args{
				ctx: context.Background(),
				user: user.User{
					Name:     "Karim Andersson",
					Document: "50215322202",
					ID:       "44332222",
					Phone:    "44332222",
				},
			},
			fields: fields{
				users: &user.ActionsMock{
					GetUserWithDocumentFunc: func(ctx context.Context, doc types.Document) (user.User, error) {
						return user.User{}, user.ErrAlreadyExists
					},
				},
			},
			wantErr: user.ErrAlreadyExists,
		},
		{
			name: "should fail for fetch error",
			args: args{
				ctx: context.Background(),
				user: user.User{
					Name:     "Karim Andersson",
					Document: "50215322202",
					ID:       "FakeID",
					Phone:    "44332222",
				},
			},
			fields: fields{
				users: &user.ActionsMock{
					GetUserWithDocumentFunc: func(ctx context.Context, doc types.Document) (user.User, error) {
						return user.User{}, user.ErrInvalidName
					},
				},
			},
			wantErr: user.ErrInvalidName,
		},
		{
			name: "should fail for invalid name",
			args: args{
				ctx: context.Background(),
				user: user.User{
					Name:     "",
					Document: "50215322202",
					ID:       "FakeID",
					Phone:    "44332222",
				},
			},
			wantErr: user.ErrInvalidName,
		},
	} {
		tc := tc

		t.Run(tc.name, func(t *testing.T) {
			t.Parallel()

			err := jobs{
				users: tc.fields.users,
			}.ValidateUser(tc.args.ctx, tc.args.user)

			assert.ErrorIs(t, err, tc.wantErr)
		})
	}
}
