package obs

import (
	"context"
	"testing"

	"github.com/sirupsen/logrus"
)

var log *logrus.Entry

type LoggerContextKey struct{}

func NewLogger(l *logrus.Entry) {
	log = l
}

func NewTestLogger(t *testing.T) {
	log = logrus.WithField("testname", t.Name())
}

func Log(ctx context.Context) *logrus.Entry {
	lg, ok := ctx.Value(LoggerContextKey{}).(*logrus.Entry)
	if !ok {
		log.Error("asked for log not set in context")
		return log
	}

	return lg
}

func LoggerToContext(ctx context.Context, entry *logrus.Entry) context.Context {
	return context.WithValue(ctx, LoggerContextKey{}, entry)
}
