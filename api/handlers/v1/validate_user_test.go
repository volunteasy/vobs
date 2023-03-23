package v1

import (
	"context"
	"encoding/json"
	"govobs/api/request"
	"govobs/api/tests"
	userdomain "govobs/core/user"
	"govobs/jobs/user"
	"govobs/obs"
	"net/http"
	"testing"

	"github.com/stretchr/testify/assert"
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
			wantBody request.Response
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
			wantBody: request.Response{
				Error: map[string]interface{}{
					"code":    "INVALID_REQUEST_BODY",
					"message": "The request body could not be parsed. Please verify if it is not offending the endpoint contract",
					"target":  "User.Document: should not be of type number",
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
			wantBody: request.Response{
				Error: map[string]interface{}{
					"code":    "ERR_USER_ALREADY_EXISTS",
					"message": "Ops! O documento informado já está cadastrado em outra conta",
				},
			},
		},
	} {
		tc := tc

		t.Run(tc.name, func(t *testing.T) {
			t.Parallel()

			rec := tests.Route(
				http.MethodPost, "/api/v1/users", false, ValidateUser(tc.fields.users))(
				tests.CreateRequestWithBody(http.MethodPost, "/api/v1/users", tc.args.user),
			)

			assert.Equal(t, tc.wantCode, rec.Code)

			var res request.Response
			err := json.Unmarshal(rec.Body.Bytes(), &res)
			if err != nil {

				t.Fatalf("error parsing response: %s \n %s", err, rec.Body.String())
			}

			assert.Equal(t, tc.wantBody, res)

		})
	}
}
