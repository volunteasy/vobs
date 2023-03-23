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

func EncodeURLParameters(params map[string]string) string {
	q := url.Values{}
	for k, v := range params {
		q.Add(k, v)
	}
	return "?" + q.Encode()
}

func CreateRequestWithBody(method, target string, body interface{}) *http.Request {
	var b *bytes.Reader
	if body != nil {
		parsed, _ := json.Marshal(body)
		b = bytes.NewReader(parsed)
	}
	return httptest.NewRequest(method, target, b)
}

func CreateRequestWithParams(method, target string, params map[string]string) *http.Request {
	route := target
	if params != nil {
		encoded := EncodeURLParameters(params)
		route += encoded
	}
	return httptest.NewRequest(method, route, nil)
}
