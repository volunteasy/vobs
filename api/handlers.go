package api

import (
	"fmt"
	"govobs/api/handlers/docs"
	v1 "govobs/api/handlers/v1"
	mid "govobs/api/middleware"
	"govobs/app"
	"net/http"

	"github.com/go-chi/chi/v5"
	"github.com/go-chi/chi/v5/middleware"
	"github.com/sirupsen/logrus"

	_ "govobs/docs/swagger"
)

type Deps struct {
	App    app.App
	Logger *logrus.Entry
}

func Handler(app app.App) http.Handler {
	router := chi.NewRouter().With(
		middleware.RequestID,
		mid.Logging(app.Logger),
		middleware.Recoverer,
	)

	router.Route("/docs", func(r chi.Router) {
		r.Mount("/swagger", docs.Swagger())
		r.Mount("/redoc", docs.Redoc())
	})

	router.Route("/api/v1", func(r chi.Router) {

		r.Route("/users", func(r chi.Router) {
			r.Post("/", v1.ValidateUser(app.Users))
			r.Get(fmt.Sprintf("/{%s}", v1.UserIDParam), v1.GetUser(app.Users))
		})
	})

	return router
}
