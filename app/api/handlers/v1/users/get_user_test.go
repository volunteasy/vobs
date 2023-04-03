package users

import (
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"net/http/httptest"
	"testing"

	"govobs/app/api/rest"
	"govobs/app/api/tests"
	"govobs/app/core/types"
	userdomain "govobs/app/core/user"
	"govobs/app/jobs/user"
	"govobs/app/obs"

	"github.com/stretchr/testify/assert"
)

func TestGetUser(t *testing.T) {
	t.Parallel()

	obs.NewTestLogger(t)

	type (
		args struct {
			userID types.ID
		}

		fields struct {
			users *user.JobsMock
		}

		test struct {
			name     string
			args     args
			fields   fields
			wantCode int
			wantBody rest.Response
		}
	)

	for _, tc := range []test{
		{
			name: "should get user successfully",
			args: args{
				userID: 565565,
			},
			fields: fields{
				users: &user.JobsMock{
					GetUserFunc: func(ctx context.Context, id types.ID) (userdomain.User, error) {
						return userdomain.User{
							ID:       565565,
							Document: "5007",
							Phone:    "3232",
							Name:     "Ni hao ma",
						}, nil
					},
				},
			},
			wantCode: http.StatusOK,
			wantBody: rest.Response{
				Data: map[string]interface{}{
					"id":       "565565",
					"document": "5007",
					"phone":    "3232",
					"name":     "Ni hao ma",
				},
			},
		},
		{
			name: "should fail with user not found",
			args: args{
				userID: 565565,
			},
			fields: fields{
				users: &user.JobsMock{
					GetUserFunc: func(ctx context.Context, id types.ID) (userdomain.User, error) {
						return userdomain.User{}, userdomain.ErrNotFound
					},
				},
			},
			wantCode: http.StatusNotFound,
			wantBody: rest.Response{
				Error: map[string]interface{}{
					"code":    "err_not_found",
					"message": "Ops! Parece que este usuário não existe",
				},
			},
		},
	} {
		tc := tc

		t.Run(tc.name, func(t *testing.T) {
			t.Parallel()

			router := tests.Router()
			rec := httptest.NewRecorder()

			Handler(router, tc.fields.users)

			router.ServeHTTP(rec,
				tests.CreateRequestWithBody(http.MethodGet, fmt.Sprintf("/%d", tc.args.userID), nil),
			)

			if assert.Len(t, tc.fields.users.GetUserCalls(), 1) {
				assert.EqualValues(t, tc.args.userID, tc.fields.users.GetUserCalls()[0].ID)
			}

			assert.Equal(t, tc.wantCode, rec.Code)

			var res rest.Response
			json.Unmarshal(rec.Body.Bytes(), &res)

			assert.Equal(t, tc.wantBody, res)
		})
	}
}
