package user

import (
	"context"
	"gotest.tools/v3/assert"
	"govobs/core/types"
	"govobs/core/user"
	"testing"
)

func TestJobs_CreateUser(t *testing.T) {
	t.Parallel()

	type (
		args struct {
			ctx  context.Context
			user user.User
		}

		fields struct {
			users    *user.ActionsMock
			createID types.IDCreator
		}

		test struct {
			name    string
			args    args
			fields  fields
			wantErr error
			wantID  types.ID
		}
	)

	for _, tc := range []test{
		{
			name: "should create user and return id",
			args: args{
				ctx: context.Background(),
				user: user.User{
					Name:     "Karim Andersson",
					Nickname: "kandss",
					Document: "50215322202",
					Contact: types.Contact{
						Phone: types.Phone{
							CountryCode: "55",
							AreaCode:    "22",
							PhoneNumber: "44332222",
						},
						Address: types.Address{
							ZipCode:     "55478770",
							HouseNumber: "33",
							StreetName:  "My Avenue",
							Complement:  "Apartment 12",
							District:    "Thirteen",
							City:        "Beijing",
							State:       "Huangzou",
							Country:     "Australia",
						},
					},
				},
			},
			fields: fields{
				users: &user.ActionsMock{
					CreateUserFunc: func(ctx context.Context, u user.User) error {
						return nil
					},
				},
				createID: func() types.ID {
					return 14
				},
			},
			wantErr: nil,
			wantID:  14,
		},
		{
			name: "should fail for persistence error",
			args: args{
				ctx: context.Background(),
				user: user.User{
					Name:     "Karim Andersson",
					Nickname: "kandss",
					Document: "50215322202",
					Contact: types.Contact{
						Phone: types.Phone{
							CountryCode: "55",
							AreaCode:    "22",
							PhoneNumber: "44332222",
						},
						Address: types.Address{
							ZipCode:     "55478770",
							HouseNumber: "33",
							StreetName:  "My Avenue",
							Complement:  "Apartment 12",
							District:    "Thirteen",
							City:        "Beijing",
							State:       "Huangzou",
							Country:     "Australia",
						},
					},
				},
			},
			fields: fields{
				users: &user.ActionsMock{
					CreateUserFunc: func(ctx context.Context, u user.User) error {
						return user.ErrInvalidName
					},
				},
				createID: func() types.ID {
					return 0
				},
			},
			wantErr: user.ErrInvalidName,
			wantID:  0,
		},
		{
			name: "should fail for nickname error",
			args: args{
				ctx: context.Background(),
				user: user.User{
					Name:     "Karim Andersson",
					Nickname: "",
					Document: "50215322202",
					Contact: types.Contact{
						Phone: types.Phone{
							CountryCode: "55",
							AreaCode:    "22",
							PhoneNumber: "44332222",
						},
						Address: types.Address{
							ZipCode:     "55478770",
							HouseNumber: "33",
							StreetName:  "My Avenue",
							Complement:  "Apartment 12",
							District:    "Thirteen",
							City:        "Beijing",
							State:       "Huangzou",
							Country:     "Australia",
						},
					},
				},
			},
			wantErr: user.ErrInvalidNickname,
			wantID:  0,
		},
		{
			name: "should fail for invalid name",
			args: args{
				ctx: context.Background(),
				user: user.User{
					Name:     "",
					Nickname: "kandss",
					Document: "50215322202",
					Contact: types.Contact{
						Phone: types.Phone{
							CountryCode: "55",
							AreaCode:    "22",
							PhoneNumber: "44332222",
						},
						Address: types.Address{
							ZipCode:     "55478770",
							HouseNumber: "33",
							StreetName:  "My Avenue",
							Complement:  "Apartment 12",
							District:    "Thirteen",
							City:        "Beijing",
							State:       "Huangzou",
							Country:     "Australia",
						},
					},
				},
			},
			wantErr: user.ErrInvalidName,
			wantID:  0,
		},
		{
			name: "should fail for invalid address",
			args: args{
				ctx: context.Background(),
				user: user.User{
					Name:     "Karim Andersson",
					Nickname: "kandss",
					Document: "50215322202",
					Contact: types.Contact{
						Phone: types.Phone{
							CountryCode: "55",
							AreaCode:    "22",
							PhoneNumber: "44332222",
						},
						Address: types.Address{
							ZipCode:     "",
							HouseNumber: "33",
							StreetName:  "My Avenue",
							Complement:  "Apartment 12",
							District:    "Thirteen",
							City:        "Beijing",
							State:       "Huangzou",
							Country:     "Australia",
						},
					},
				},
			},
			wantErr: types.ErrInvalidAddress,
			wantID:  0,
		},
		{
			name: "should fail for invalid phone",
			args: args{
				ctx: context.Background(),
				user: user.User{
					Name:     "Karim Andersson",
					Nickname: "kandss",
					Document: "50215322202",
					Contact: types.Contact{
						Phone: types.Phone{
							CountryCode: "55",
							AreaCode:    "22",
							PhoneNumber: "",
						},
						Address: types.Address{
							ZipCode:     "55478770",
							HouseNumber: "33",
							StreetName:  "My Avenue",
							Complement:  "Apartment 12",
							District:    "Thirteen",
							City:        "Beijing",
							State:       "Huangzou",
							Country:     "Australia",
						},
					},
				},
			},
			wantErr: types.ErrInvalidPhone,
			wantID:  0,
		},
	} {
		tc := tc

		t.Run(tc.name, func(t *testing.T) {
			t.Parallel()

			u, err := jobs{
				users:    tc.fields.users,
				createID: tc.fields.createID,
			}.CreateUser(tc.args.ctx, tc.args.user)

			assert.ErrorIs(t, err, tc.wantErr)
			assert.Equal(t, tc.wantID, u.ID)
		})
	}
}
