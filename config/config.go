package config

type (
	Config struct {
		Name        string `envconfig:"APP_NAME" default:"govobs"`
		Environment bool   `envconfig:"ENVIRONMENT" default:"development"`

		API    API
		Logger Logger
	}

	API struct {
		Port string `envconfig:"LISTEN_PORT" required:"true"`
	}

	Logger struct {
		DSN string `envconfig:"DSN" required:"true"`
	}
)
