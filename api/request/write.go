package request

import (
	"encoding/json"
	"net/http"
)

func Write(wr http.ResponseWriter, r Response) (err error) {
	code, header := r.code, wr.Header()

	for k, v := range r.headers {
		header.Set(k, v)
	}

	if code == 0 {
		code = 200
	}

	wr.WriteHeader(code)

	return encodeJSON(wr, r)
}

func encodeJSON(wr http.ResponseWriter, data interface{}) error {
	enc := json.NewEncoder(wr)

	err := enc.Encode(data)
	if err == nil {
		return nil
	}

	jsonErr := enc.Encode(InternalServerError)
	if jsonErr != nil {
		return jsonErr
	}

	return err
}
