package users

import (
	"context"
	"encoding/json"
	"net/http"
	"net/http/httptest"
	"testing"

	"github.com/stretchr/testify/assert"
	"govobs/app/api/rest"
	"govobs/app/api/tests"
	userdomain "govobs/app/core/user"
	"govobs/app/jobs/user"
	"govobs/app/obs"
)

func TestValidateUser(t *testing.T) {
	t.Parallel()

	obs.NewTestLogger(t)

	type (
		args struct {
			user map[string]interface{}
		}

		fields struct {
			users user.Jobs
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
			name: "should validate user successfully",
			args: args{
				user: map[string]interface{}{
					"document": "49333205605",
					"name":     "Enots Ecitla",
					"id":       "hwuriKWdjevn34567p",
					"phone":    "8855332251561",
				},
			},
			fields: fields{
				users: &user.JobsMock{
					ValidateUserFunc: func(ctx context.Context, u userdomain.User) error {
						return nil
					},
				},
			},
			wantCode: http.StatusNoContent,
		},
		{
			name: "should fail for invalid body",
			args: args{
				user: map[string]interface{}{
					"document": 49333205605,
					"name":     "Enots Ecitla",
					"id":       "hwuriKWdjevn34567p",
					"phone":    "8855332251561",
				},
			},
			wantCode: http.StatusBadRequest,
			wantBody: rest.Response{
				Error: map[string]interface{}{
					"code":    "invalid_request_body",
					"message": "User.document: should not be of type number",
				},
			},
		},
		{
			name: "should fail because user already exists",
			args: args{
				user: map[string]interface{}{
					"document": "49333205605",
					"name":     "Enots Ecitla",
					"id":       "hwuriKWdjevn34567p",
					"phone":    "8855332251561",
				},
			},
			fields: fields{
				users: &user.JobsMock{
					ValidateUserFunc: func(ctx context.Context, u userdomain.User) error {
						return userdomain.ErrAlreadyExists
					},
				},
			},
			wantCode: http.StatusPreconditionFailed,
			wantBody: rest.Response{
				Error: map[string]interface{}{
					"code":    "err_already_exists",
					"message": "Ops, esse documento já está sendo usado por outra conta",
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
				tests.CreateRequestWithBody(http.MethodPost, "/", tc.args.user),
			)

			assert.Equal(t, tc.wantCode, rec.Code)

			var res rest.Response
			json.Unmarshal(rec.Body.Bytes(), &res)

			assert.Equal(t, tc.wantBody, res)
		})
	}
}
