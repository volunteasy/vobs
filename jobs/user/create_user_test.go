package user

import (
	"context"
	"govobs/core/types"
	"govobs/core/user"
	"testing"

	"github.com/stretchr/testify/assert"
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
			want    user.User
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
			want: user.User{
				ID:       14,
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
			want:    user.User{},
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
			want:    user.User{},
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
			want:    user.User{},
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
			assert.Equal(t, tc.want, u)
		})
	}
}
