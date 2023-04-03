package organization

import (
	"context"
	"testing"

	"govobs/app/core/organization"
	"govobs/app/core/types"
	"govobs/app/providers/mysql/tests"

	"github.com/stretchr/testify/assert"
)

func TestGetOrganization(t *testing.T) {
	t.Parallel()

	type (
		args struct {
			id types.ID
		}

		test struct {
			name    string
			args    args
			want    organization.Organization
			wantErr error
		}
	)

	for _, tc := range []test{
		{
			name: "should get organization successfully",
			args: args{
				id: 1,
			},
			want: organization.Organization{
				ID:       1,
				Name:     "Helping Hands",
				Document: "12345678900",
				Contact: types.Contact{
					Phone: "5511999999999",
					Address: types.Address{
						ZipCode:     "01001-000",
						HouseNumber: "123",
						StreetName:  "Rua da Ajuda",
						Complement:  "Sala 1001",
						District:    "Centro",
						City:        "São Paulo",
						State:       "SP",
						Country:     "Brasil",
					},
				},
			},
		},
		{
			name: "should fail for non existing organization",
			args: args{
				id: 32,
			},
			wantErr: organization.ErrNotFound,
		},
	} {
		tc := tc

		t.Run(tc.name, func(t *testing.T) {
			t.Parallel()

			org, err := actions{db: tests.NewDatabase(t)}.
				GetOrganization(context.Background(), tc.args.id)

			assert.Equal(t, tc.want, org)
			assert.ErrorIs(t, err, tc.wantErr)
		})

	}
}
