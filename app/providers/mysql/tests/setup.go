package tests

import (
	"os"
	"testing"
)

func SetupTest(m *testing.M) {
	docker, err := NewPool()
	if err != nil {
		panic(err.Error())
	}

	purge, err := MySQL(docker)
	if err != nil {
		panic(err.Error())
	}

	defer purge()

	os.Exit(m.Run())
}
