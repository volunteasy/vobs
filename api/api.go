package api

import (
	"context"
	"fmt"
	"net"
	"net/http"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/sirupsen/logrus"
)

func NewServer(writeTimeout, readTimeout time.Duration, log logrus.FieldLogger) func(lis net.Listener, handler http.Handler) {
	return func(lis net.Listener, handler http.Handler) {
		//nolint:gosec
		srv := &http.Server{
			Handler:      handler,
			WriteTimeout: writeTimeout,
			ReadTimeout:  readTimeout,
		}

		c := make(chan os.Signal, 1)
		idleConnections := make(chan struct{})
		signal.Notify(c, os.Interrupt, syscall.SIGINT)

		go func() {
			<-c
			// create context with timeout
			ctx, cancel := context.WithTimeout(context.Background(), writeTimeout)
			defer cancel()

			// start http shutdown
			if err := srv.Shutdown(ctx); err != nil {
				log.WithError(err).Error("Shutting down")
			}

			close(idleConnections)
		}()

		log.Info(fmt.Sprintf("Listening at %s", lis.Addr().String()))
		if err := srv.Serve(lis); err != nil && err != http.ErrServerClosed {
			log.WithError(err).Fatal("Listen and serve failed")
		}

		log.Info("Waiting idle connections...")
		<-idleConnections

		log.Info("Au revoir")
	}
}
