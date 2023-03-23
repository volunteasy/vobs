package request

type Error struct {
	Code    string      `json:"code"`
	Message string      `json:"message"`
	Target  interface{} `json:"target,omitempty"`
	status  int
}

type Response struct {
	Error   interface{} `json:"error"`
	Data    interface{} `json:"data"`
	err     error
	code    int
	headers map[string]string
}
