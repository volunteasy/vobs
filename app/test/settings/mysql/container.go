package mysql

import (
	"fmt"

	"github.com/ory/dockertest"
	"github.com/ory/dockertest/docker"
	"govobs/app/config"
)

const (
	containerName = "mysql_volunteasy_testing_container"
	expires       = 600
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

	err = d.Retry(func() error {
		container, err := useContainer(d, containerName)
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

func Startup(d *dockertest.Resource) (DatabaseContextBuilder, error) {
	port := d.GetPort("3306/tcp")

	conn, _, err := useConnection(config.MySQL{
		DSN: fmt.Sprintf("root:volunteasy@tcp(localhost:%s)/%s?multiStatements=true", port, "volunteasytst"),
	})
	if err != nil {
		return nil, fmt.Errorf("an error occurred when pinging database: %w", err)
	}

	return builder(conn, port), nil
}

func useContainer(d *dockertest.Pool, name string) (*dockertest.Resource, error) {
	container, err := d.Client.InspectContainer(name)
	if container == nil {
		return nil, err
	}

	return &dockertest.Resource{Container: container}, nil
}
