package request

type ResponseOptions func(response Response) Response

func WithHeaders(m map[string]string) ResponseOptions {
	return func(r Response) Response {
		r.headers = m
		return r
	}
}

func WithHeader(key, value string) ResponseOptions {
	return func(r Response) Response {
		if r.headers == nil {
			r.headers = map[string]string{key: value}
		}
		r.headers[key] = value
		return r
	}
}

func WithError(err error) ResponseOptions {
	return func(r Response) Response {
		r.err = err
		return r
	}
}

func WithStatus(status int) ResponseOptions {
	return func(r Response) Response {
		r.code = status
		return r
	}
}
