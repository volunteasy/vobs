package telemetry

import (
	"context"
	"github.com/sirupsen/logrus"
)

var log *logrus.Entry

func Log() *logrus.Entry {
	return log
}

type LoggerContextKey struct {
}

func NewLogger(l *logrus.Entry) {
	log = l
}

func LoggerToContext(ctx context.Context, entry *logrus.Entry) context.Context {
	return context.WithValue(ctx, LoggerContextKey{}, entry)
}

func LoggerFromContext(ctx context.Context) *logrus.Entry {
	return ctx.Value(LoggerContextKey{}).(*logrus.Entry)
}
