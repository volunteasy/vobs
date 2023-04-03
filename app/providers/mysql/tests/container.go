package tests

import (
	"fmt"

	"govobs/app/config"
	"govobs/app/providers/mysql/conn"

	"github.com/ory/dockertest"
	"github.com/ory/dockertest/docker"
	"github.com/pkg/errors"
)

const (
	containerName = "mysql_volunteasy_testing_container"
	expires       = 600
	dsnTemplate   = "root:volunteasy@tcp(localhost:%s)/"
)

var options = dockertest.RunOptions{
	Name:       containerName,
	Repository: "mysql",
	Tag:        "8.0",
	Env: []string{
		"MYSQL_USER=volunteasy",
		"MYSQL_PASSWORD=volunteasy",
		"MYSQL_DATABASE=volunteasytst",
		"MYSQL_ROOT_PASSWORD=volunteasy",
	},
}

var dsn string

func DSN() string {
	return dsn
}

func Create(d *dockertest.Pool) (*dockertest.Resource, error) {
	container, _ := useContainer(d, containerName)
	if container != nil && container.Container.State.Running {
		return container, nil
	}

	if container != nil && !container.Container.State.Running {
		_ = d.RemoveContainerByName(containerName)
	}

	container, err := d.RunWithOptions(&options, func(c *docker.HostConfig) {
		c.AutoRemove = true
		c.RestartPolicy = docker.RestartPolicy{Name: "no"}
	})

	if err == nil {
		container.Expire(expires)
		return container, nil
	}

	err = d.Retry(func() (err error) {
		container, err = useContainer(d, containerName)
		if container != nil {
			return nil
		}

		return err
	})

	if err != nil || container == nil {
		return nil, fmt.Errorf(`failed getting container resource: %w`, err)
	}

	return container, nil
}

func Startup(d *dockertest.Resource) error {
	dsn = fmt.Sprintf(dsnTemplate, d.GetPort("3306/tcp"))

	conn, err := conn.NewConnection(config.MySQL{
		DSN: dsn,
	})
	if err != nil {
		return fmt.Errorf("an error occurred when pinging database: %w", err)
	}

	conn.Close()

	return nil
}

func MySQL(d *dockertest.Pool) (purge func(), err error) {
	res, err := Create(d)
	if err != nil {
		if res != nil {
			defer d.Purge(res)
		}

		return nil, err
	}

	err = d.Retry(func() (err error) {
		return Startup(res)
	})

	if err != nil {
		defer d.Purge(res)
		return nil, err
	}

	return func() {
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

func useContainer(d *dockertest.Pool, name string) (*dockertest.Resource, error) {
	container, err := d.Client.InspectContainer(name)
	if container == nil {
		return nil, err
	}

	return &dockertest.Resource{Container: container}, nil
}
