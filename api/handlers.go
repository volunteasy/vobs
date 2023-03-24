package api

import (
	"govobs/api/handlers/docs"
	"govobs/api/handlers/v1/users"
	"govobs/api/middleware"
	"govobs/app"
	"net/http"

	"github.com/go-chi/chi/v5"
	"github.com/sirupsen/logrus"

	_ "govobs/docs/swagger"
)

type Deps struct {
	App    app.App
	Logger *logrus.Entry
}

func Handler(app app.App) http.Handler {
	router := chi.NewRouter().With(
		middleware.RequestID(app.IDs),
		middleware.Logging(app.Logger),
	)

	router.Get("/", func(w http.ResponseWriter, r *http.Request) {
		w.Write([]byte("API is running"))
	})

	router.Route("/docs", func(r chi.Router) {
		r.Mount("/swagger", docs.Swagger())
		r.Mount("/redoc", docs.Redoc())
	})

	router.Route("/api/v1", func(r chi.Router) {

		r.Route("/users", func(r chi.Router) {
			users.Handler(r, app.Users)
		})
	})

	return router
}
