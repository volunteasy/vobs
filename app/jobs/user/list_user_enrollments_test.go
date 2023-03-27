package user

import (
	"context"
	"testing"

	"github.com/stretchr/testify/assert"
	"govobs/app/core/organization"
	"govobs/app/core/types"
)

func TestJobs_ListUserEnrollments(t *testing.T) {
	t.Parallel()

	type (
		args struct {
			ctx    context.Context
			userID types.UserID
			filter organization.Filter
		}

		fields struct {
			organizations *organization.ActionsMock
		}

		test struct {
			name      string
			args      args
			fields    fields
			wantErr   error
			want      []organization.Enrollment
			wantCount int
		}
	)

	for _, tc := range []test{
		{
			name: "should list benefits",
			args: args{
				ctx:    context.Background(),
				userID: "23",
				filter: organization.Filter{},
			},
			fields: fields{
				organizations: &organization.ActionsMock{
					ListEnrollmentsFunc: func(ctx context.Context, userID types.UserID, f organization.Filter) ([]organization.Enrollment, int, error) {
						return []organization.Enrollment{
							{
								Organization: organization.Organization{
									ID:       34,
									Name:     "Hefty",
									Document: "10555848000155",
									Contact: types.Contact{
										Phone: "44332222",

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
						}, 10, nil
					},
				},
			},

			wantErr: nil,
			want: []organization.Enrollment{
				{
					Organization: organization.Organization{
						ID:       34,
						Name:     "Hefty",
						Document: "10555848000155",
						Contact: types.Contact{
							Phone: "44332222",
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
			},
			wantCount: 10,
		},
	} {
		tc := tc

		t.Run(tc.name, func(t *testing.T) {
			t.Parallel()

			benefits, count, err := jobs{
				organizations: tc.fields.organizations,
			}.ListUserEnrollments(tc.args.ctx, tc.args.userID, tc.args.filter)

			assert.ErrorIs(t, err, tc.wantErr)
			assert.Equal(t, tc.want, benefits)
			assert.Equal(t, tc.wantCount, count)
		})
	}
}
