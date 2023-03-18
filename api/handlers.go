package api

import (
	"fmt"
	v1 "govobs/api/handlers/v1"
	mid "govobs/api/middleware"
	"net/http"

	"github.com/go-chi/chi/v5"
	"github.com/go-chi/chi/v5/middleware"
	"github.com/sirupsen/logrus"
)

type Deps struct {
	Logger *logrus.Entry
}

func Handler(deps Deps) http.Handler {
	router := chi.NewRouter()

	router.Use(
		middleware.RequestID,
		mid.Logging(deps.Logger),
		middleware.Recoverer,
	)

	router.Route("/api/v1", func(r chi.Router) {
		r.Route("/users", func(r chi.Router) {
			r.Post("/", v1.CreateUser(nil))
			r.Get(fmt.Sprintf("/{%s}", v1.UserIDParam), v1.GetUser(nil))
		})
	})

	return router
}
