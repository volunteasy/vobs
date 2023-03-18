package send

import (
	"net/http"
)

func Write(wr http.ResponseWriter, r Response) (err error) {
	code, header := r.Code, wr.Header()

	for k, v := range r.Header {
		header.Set(k, v)
	}

	wr.WriteHeader(code)

	if r.Data != nil {
		err = encodeJSON(wr, r.Data)
	}

	return
}
