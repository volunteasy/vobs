package user
//
//import (
//    "context"
//    "govobs/core/benefit"
//    "govobs/core/organization"
//    "govobs/core/types"
//    "testing"
//)
//
//func TestJobs_ListUserBenefits(t *testing.T) {
//	t.Parallel()
//
//	type (
//		args struct {
//			ctx  context.Context
//			userID types.ID
//			filter benefit.Filter
//		}
//
//		fields struct {
//			benefits *benefit.ActionsMock
//		}
//
//		test struct {
//			name    string
//			args    args
//			fields  fields
//			wantErr error
//			want []benefit.Benefit
//			wantCount int
//		}
//	)
//
//	for _, tc := range []test{
//		{
//			name: "should list benefits",
//			args: args{
//				ctx: context.Background(),
//				userID: 23,
//				filter: benefit.Filter{
//                    ClaimDateRange: types.DateRange{},
//                    AssistedID:     0,
//                },
//            },
//			fields: fields{
//				users: &user.ActionsMock{
//					CreateUserFunc: func(ctx context.Context, u user.User) error {
//						return nil
//					},
//				},
//				createID: func() types.ID {
//					return 14
//				},
//			},
//			wantErr: nil,
//			wantID:  14,
//		},
//		{
//			name: "should fail for persistence error",
//			args: args{
//				ctx: context.Background(),
//				user: user.User{
//					Name:     "Karim Andersson",
//					Nickname: "kandss",
//					Document: "50215322202",
//					Contact: types.Contact{
//						Phone: types.Phone{
//							CountryCode: "55",
//							AreaCode:    "22",
//							PhoneNumber: "44332222",
//						},
//						Address: types.Address{
//							ZipCode:     "55478770",
//							HouseNumber: "33",
//							StreetName:  "My Avenue",
//							Complement:  "Apartment 12",
//							District:    "Thirteen",
//							City:        "Beijing",
//							State:       "Huangzou",
//							Country:     "Australia",
//						},
//					},
//				},
//			},
//			fields: fields{
//				users: &user.ActionsMock{
//					CreateUserFunc: func(ctx context.Context, u user.User) error {
//						return user.ErrInvalidName
//					},
//				},
//				createID: func() types.ID {
//					return 0
//				},
//			},
//			wantErr: user.ErrInvalidName,
//			wantID:  0,
//		},
//		{
//			name: "should fail for nickname error",
//			args: args{
//				ctx: context.Background(),
//				user: user.User{
//					Name:     "Karim Andersson",
//					Nickname: "",
//					Document: "50215322202",
//					Contact: types.Contact{
//						Phone: types.Phone{
//							CountryCode: "55",
//							AreaCode:    "22",
//							PhoneNumber: "44332222",
//						},
//						Address: types.Address{
//							ZipCode:     "55478770",
//							HouseNumber: "33",
//							StreetName:  "My Avenue",
//							Complement:  "Apartment 12",
//							District:    "Thirteen",
//							City:        "Beijing",
//							State:       "Huangzou",
//							Country:     "Australia",
//						},
//					},
//				},
//			},
//			wantErr: user.ErrInvalidNickname,
//			wantID:  0,
//		},
//		{
//			name: "should fail for invalid name",
//			args: args{
//				ctx: context.Background(),
//				user: user.User{
//					Name:     "",
//					Nickname: "kandss",
//					Document: "50215322202",
//					Contact: types.Contact{
//						Phone: types.Phone{
//							CountryCode: "55",
//							AreaCode:    "22",
//							PhoneNumber: "44332222",
//						},
//						Address: types.Address{
//							ZipCode:     "55478770",
//							HouseNumber: "33",
//							StreetName:  "My Avenue",
//							Complement:  "Apartment 12",
//							District:    "Thirteen",
//							City:        "Beijing",
//							State:       "Huangzou",
//							Country:     "Australia",
//						},
//					},
//				},
//			},
//			wantErr: user.ErrInvalidName,
//			wantID:  0,
//		},
//		{
//			name: "should fail for invalid address",
//			args: args{
//				ctx: context.Background(),
//				user: user.User{
//					Name:     "Karim Andersson",
//					Nickname: "kandss",
//					Document: "50215322202",
//					Contact: types.Contact{
//						Phone: types.Phone{
//							CountryCode: "55",
//							AreaCode:    "22",
//							PhoneNumber: "44332222",
//						},
//						Address: types.Address{
//							ZipCode:     "",
//							HouseNumber: "33",
//							StreetName:  "My Avenue",
//							Complement:  "Apartment 12",
//							District:    "Thirteen",
//							City:        "Beijing",
//							State:       "Huangzou",
//							Country:     "Australia",
//						},
//					},
//				},
//			},
//			wantErr: types.ErrInvalidAddress,
//			wantID:  0,
//		},
//		{
//			name: "should fail for invalid phone",
//			args: args{
//				ctx: context.Background(),
//				user: user.User{
//					Name:     "Karim Andersson",
//					Nickname: "kandss",
//					Document: "50215322202",
//					Contact: types.Contact{
//						Phone: types.Phone{
//							CountryCode: "55",
//							AreaCode:    "22",
//							PhoneNumber: "",
//						},
//						Address: types.Address{
//							ZipCode:     "55478770",
//							HouseNumber: "33",
//							StreetName:  "My Avenue",
//							Complement:  "Apartment 12",
//							District:    "Thirteen",
//							City:        "Beijing",
//							State:       "Huangzou",
//							Country:     "Australia",
//						},
//					},
//				},
//			},
//			wantErr: types.ErrInvalidAddress,
//			wantID:  0,
//		},
//	} {
//		tc := tc
//
//		t.Run(tc.name, func(t *testing.T) {
//			t.Parallel()
//
//			u, err := jobs{
//				users:    tc.fields.users,
//				createID: tc.fields.createID,
//			}.CreateUser(tc.args.ctx, tc.args.user)
//
//			assert.ErrorIs(t, err, tc.wantErr)
//			assert.Equal(t, tc.wantID, u.ID)
//		})
//	}
//}
