package users

import (
	"context"
	"encoding/json"
	"govobs/api/rest"
	"govobs/api/tests"
	"govobs/core/types"
	userdomain "govobs/core/user"
	"govobs/jobs/user"
	"govobs/obs"
	"net/http"
	"net/http/httptest"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestGetUser(t *testing.T) {
	t.Parallel()

	obs.NewTestLogger(t)

	type (
		args struct {
			userID string
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
				userID: "hwuriKWdjevn34567p",
			},
			fields: fields{
				users: &user.JobsMock{
					GetUserFunc: func(ctx context.Context, id types.UserID) (userdomain.User, error) {
						return userdomain.User{
							ID:       "hwuriKWdjevn34567p",
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
					"id":       "hwuriKWdjevn34567p",
					"document": "5007",
					"phone":    "3232",
					"name":     "Ni hao ma",
				},
			},
		},
		{
			name: "should fail with user not found",
			args: args{
				userID: "hwuriKWdjevn34567p",
			},
			fields: fields{
				users: &user.JobsMock{
					GetUserFunc: func(ctx context.Context, id types.UserID) (userdomain.User, error) {
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
				tests.CreateRequestWithBody(http.MethodGet, "/"+tc.args.userID, nil),
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
