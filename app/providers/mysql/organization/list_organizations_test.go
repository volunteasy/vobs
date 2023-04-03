package organization

import (
	"context"
	"testing"

	"govobs/app/core/organization"
	"govobs/app/core/types"
	"govobs/app/providers/mysql/tests"

	"github.com/stretchr/testify/assert"
)

func TestListOrganizations(t *testing.T) {
	t.Parallel()

	type (
		args struct {
			filter organization.Filter
		}

		test struct {
			name      string
			args      args
			want      []organization.Organization
			wantCount int
			wantErr   error
		}
	)

	for _, tc := range []test{
		{
			name: "should list organizations successfully",
			args: args{
				filter: organization.Filter{
					Filter: types.Filter{
						Range: types.ListRange{
							Start: 1, End: 2,
						},
					},
				},
			},
			wantCount: 2,
			want: []organization.Organization{
				{
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
				{
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
		{
			name: "should list organizations with name filter successfully",
			args: args{
				filter: organization.Filter{
					Name: "united",
					Filter: types.Filter{
						Range: types.ListRange{
							Start: 0, End: 2,
						},
					},
				},
			},
			wantCount: 1,
			want: []organization.Organization{
				{
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
		{
			name: "should list no organizations with name filter successfully",
			args: args{
				filter: organization.Filter{
					Name: "united people",
					Filter: types.Filter{
						Range: types.ListRange{
							Start: 0, End: 1,
						},
					},
				},
			},
			wantCount: 0,
			want:      []organization.Organization{},
		},
	} {
		tc := tc

		t.Run(tc.name, func(t *testing.T) {
			t.Parallel()

			org, count, err := actions{db: tests.NewDatabase(t)}.
				ListOrganizations(context.Background(), tc.args.filter)

			assert.Equal(t, tc.wantCount, count)
			assert.Equal(t, tc.want, org)
			assert.ErrorIs(t, err, tc.wantErr)
		})

	}
}
