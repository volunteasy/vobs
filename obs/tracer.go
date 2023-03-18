package obs

import "github.com/newrelic/go-agent/v3/newrelic"

var nr *newrelic.Application

func Tracer() *newrelic.Application {
	return nr
}

func NewTracer(n *newrelic.Application) {
	nr = n
}
