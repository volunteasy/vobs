package settings

import (
	"govobs/app/test/settings/mysql"

	"github.com/ory/dockertest"
	"github.com/pkg/errors"
)

var DBTest mysql.DatabaseContextBuilder

func MySQL(d *dockertest.Pool) (builder mysql.DatabaseContextBuilder, purge func(), err error) {
	res, err := mysql.Create(d)
	if err != nil {
		if res != nil {
			defer d.Purge(res)
		}

		return nil, nil, err
	}

	err = d.Retry(func() (err error) {
		builder, err = mysql.Startup(res)
		return err
	})

	if err != nil {
		defer d.Purge(res)
		return nil, nil, err
	}

	DBTest = builder

	return builder, func() {
		d.Purge(res)
	}, nil
}

func NewPool() (*dockertest.Pool, error) {
	pool, err := dockertest.NewPool("")
	if err != nil {
		return nil, errors.Wrap(err, "the docker pool connection could not be established")
	}

	if err := pool.Client.Ping(); err != nil {
		return nil, errors.Wrap(err, "could not contact docker pool")
	}

	return pool, nil
}
