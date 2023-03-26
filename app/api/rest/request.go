package rest

import (
	"encoding/json"
	"govobs/app/core/types"
	"net/http"

	"github.com/go-chi/chi/v5"
)

type Request struct {
	*http.Request
}

func (r Request) Param(name string) string {
	return chi.URLParam(r.Request, name)
}

func (r Request) ID(name string) (types.ID, error) {
	return types.FromString(r.Param(name))
}

func (r Request) JSONBody(i interface{}) error {
	return json.NewDecoder(r.Body).Decode(i)
}

func (r Request) GetQuery(name string) string {
	return r.URL.Query().Get(name)
}
