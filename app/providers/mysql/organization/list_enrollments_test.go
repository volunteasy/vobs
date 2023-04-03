package organization

import (
	"context"
	"testing"

	"govobs/app/core/membership"
	"govobs/app/core/organization"
	"govobs/app/core/types"
	"govobs/app/providers/mysql/tests"

	"github.com/stretchr/testify/assert"
)

func TestListEnrollments(t *testing.T) {
	t.Parallel()

	type (
		args struct {
			userID types.ID
			filter organization.Filter
		}

		test struct {
			name      string
			args      args
			want      []organization.Enrollment
			wantCount int
			wantErr   error
		}
	)

	for _, tc := range []test{
		{
			name: "should list organizations successfully",
			args: args{
				userID: 3,
				filter: organization.Filter{
					Filter: types.Filter{
						Range: types.ListRange{
							Start: 0, End: 2,
						},
					},
				},
			},
			wantCount: 2,
			want: []organization.Enrollment{
				{
					Role:   membership.RoleAssisted,
					Status: membership.StatusAccepted,
					Organization: organization.Organization{
						ID:       2,
						Name:     "Volunteer Corps",
						Document: "98765432100",
						Contact: types.Contact{
							Phone: "5511888888888",
							Address: types.Address{
								ZipCode:     "02002-000",
								HouseNumber: "456",
								StreetName:  "Rua do Voluntariado",
								Complement:  "Andar 5",
								District:    "Pinheiros",
								City:        "São Paulo",
								State:       "SP",
								Country:     "Brasil",
							},
						},
					},
				},
				{
					Role:   membership.RoleVolunteer,
					Status: membership.StatusPending,
					Organization: organization.Organization{
						ID:       3,
						Name:     "People United",
						Document: "45678901200",
						Contact: types.Contact{
							Phone: "5511777777777",
							Address: types.Address{
								ZipCode:     "03003-000",
								HouseNumber: "789",
								StreetName:  "Rua da União",
								Complement:  "Loja 3",
								District:    "Vila Mariana",
								City:        "São Paulo",
								State:       "SP",
								Country:     "Brasil",
							},
						},
					},
				},
			},
		},
	} {
		tc := tc

		t.Run(tc.name, func(t *testing.T) {
			t.Parallel()

			org, count, err := actions{db: tests.NewDatabase(t)}.
				ListEnrollments(context.Background(), tc.args.userID, tc.args.filter)

			assert.Equal(t, tc.wantCount, count)
			assert.Equal(t, tc.want, org)
			assert.ErrorIs(t, err, tc.wantErr)
		})

	}
}
