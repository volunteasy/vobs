package user

import (
	"context"
	"govobs/core/benefit"
	"govobs/core/types"
	"testing"
	"time"

	"github.com/stretchr/testify/assert"
)

func TestJobs_ListUserBenefits(t *testing.T) {
	t.Parallel()

	type (
		args struct {
			ctx    context.Context
			userID types.UserID
			filter benefit.Filter
		}

		fields struct {
			benefits *benefit.ActionsMock
		}

		test struct {
			name      string
			args      args
			fields    fields
			wantErr   error
			want      []benefit.Benefit
			wantCount int
		}
	)

	for _, tc := range []test{
		{
			name: "should list benefits",
			args: args{
				ctx:    context.Background(),
				userID: "23",
				filter: benefit.Filter{
					ClaimDateRange: types.DateRange{},
				},
			},
			fields: fields{
				benefits: &benefit.ActionsMock{
					ListBenefitsFunc: func(ctx context.Context, f benefit.Filter) ([]benefit.Benefit, int, error) {
						return []benefit.Benefit{
							{
								ID:             500,
								AssistedID:     "200",
								DistributionID: 22,
								ClaimedAt:      time.Date(2015, 10, 21, 12, 22, 00, 00, time.UTC),
							},
							{
								ID:             501,
								AssistedID:     "200",
								DistributionID: 678,
								ClaimedAt:      time.Date(2015, 10, 21, 12, 22, 00, 00, time.UTC),
							},
						}, 10, nil
					},
				},
			},

			wantErr: nil,
			want: []benefit.Benefit{
				{
					ID:             500,
					AssistedID:     "200",
					DistributionID: 22,
					ClaimedAt:      time.Date(2015, 10, 21, 12, 22, 00, 00, time.UTC),
				},
				{
					ID:             501,
					AssistedID:     "200",
					DistributionID: 678,
					ClaimedAt:      time.Date(2015, 10, 21, 12, 22, 00, 00, time.UTC),
				},
			},
			wantCount: 10,
		},
	} {
		tc := tc

		t.Run(tc.name, func(t *testing.T) {
			t.Parallel()

			benefits, count, err := jobs{
				benefits: tc.fields.benefits,
			}.ListUserBenefits(tc.args.ctx, tc.args.userID, tc.args.filter)

			assert.ErrorIs(t, err, tc.wantErr)
			assert.Equal(t, tc.want, benefits)
			assert.Equal(t, tc.wantCount, count)
		})
	}
}
