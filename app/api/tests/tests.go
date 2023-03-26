package tests

import (
	"bytes"
	"encoding/json"
	"net/http"
	"net/http/httptest"
	"net/url"

	"github.com/go-chi/chi/v5"
)

func Route(method, pattern string, auth bool, h http.HandlerFunc) func(r *http.Request) *httptest.ResponseRecorder {
	rec := httptest.NewRecorder()

	router := chi.NewRouter()
	router.Method(method, pattern, h)

	return func(r *http.Request) *httptest.ResponseRecorder {
		router.ServeHTTP(rec, r)
		return rec
	}
}

func Router() chi.Router {
	return chi.NewRouter()
}

func EncodeURLParameters(params map[string]string) string {
	q := url.Values{}
	for k, v := range params {
		q.Add(k, v)
	}
	return "?" + q.Encode()
}

func CreateRequestWithBody(method, target string, body interface{}) *http.Request {
	if body == nil {
		httptest.NewRequest(method, target, nil)
	}

	parsed, _ := json.Marshal(body)
	return httptest.NewRequest(method, target, bytes.NewReader(parsed))
}

func CreateRequestWithParams(method, target string, params map[string]string) *http.Request {
	route := target
	if params != nil {
		encoded := EncodeURLParameters(params)
		route += encoded
	}
	return httptest.NewRequest(method, route, nil)
}
