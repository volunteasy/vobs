package request

import (
	"encoding/json"
	"github.com/go-chi/chi/v5"
	"github.com/newrelic/go-agent/v3/newrelic"
	"govobs/core/types"
	"net/http"
)

type Request struct {
	*http.Request
	Txn *newrelic.Transaction
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
