package organization

import (
	"context"
	"govobs/app/core/organization"
	"govobs/app/core/types"
	"govobs/app/providers/sql/transaction"
	"govobs/app/test/settings"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestCreateOrganization(t *testing.T) {
	t.Parallel()

	type (
		args struct {
			org organization.Organization
		}

		test struct {
			name    string
			args    args
			wantErr error
		}
	)

	for _, tc := range []test{
		{
			name: "should create organization successfully",
			args: args{
				org: organization.Organization{
					ID:       34,
					Name:     "Alimentando necessidades",
					Document: "87555457000155",
					Contact: types.Contact{
						Phone: "+5511999885544",
						Address: types.Address{
							ZipCode:     "03939111",
							HouseNumber: "77",
							StreetName:  "Rua da Casa",
							Complement:  "Xeks",
							District:    "Almados",
							City:        "Xiexie",
							State:       "Xie",
							Country:     "Xi",
						},
					},
				},
			},
		},
		{
			name: "should fail for existing organization with the document",
			args: args{
				org: organization.Organization{
					ID:       34,
					Name:     "Alimentando necessidades",
					Document: "12345678900",
					Contact: types.Contact{
						Phone: "+5511999885544",
						Address: types.Address{
							ZipCode:     "03939111",
							HouseNumber: "77",
							StreetName:  "Rua da Casa",
							Complement:  "Xeks",
							District:    "Almados",
							City:        "Xiexie",
							State:       "Xie",
							Country:     "Xi",
						},
					},
				},
			},
			wantErr: organization.ErrAlreadyExists,
		},
	} {
		tc := tc

		t.Run(tc.name, func(t *testing.T) {
			ctx, closeTX := transaction.NewTransactionOpener(settings.DBTest(t))(context.Background())
			defer closeTX(ctx)

			err := actions{}.CreateOrganization(ctx, tc.args.org)
			assert.ErrorIs(t, err, tc.wantErr)
		})

	}
}
