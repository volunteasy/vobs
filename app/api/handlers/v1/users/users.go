package users

import (
	"fmt"

	"github.com/go-chi/chi/v5"
	"govobs/app/jobs/user"
)

const (
	UserIDParam = "userID"
)

type Users = user.Jobs

func Handler(r chi.Router, jobs user.Jobs) {
	r.Post("/", validateUser(jobs))
	r.Get(fmt.Sprintf("/{%s}", UserIDParam), getUser(jobs))
}
