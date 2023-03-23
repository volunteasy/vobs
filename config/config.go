package config

type (
	Config struct {
		Name        string `envconfig:"APP_NAME" default:"govobs"`
		Environment string `envconfig:"ENVIRONMENT" default:"development"`

		API   API
		MySQL MySQL
	}

	API struct {
		Port string `envconfig:"PORT" required:"true"`
	}

	MySQL struct {
		Host     string `envconfig:"HOST" required:"true"`
		User     string `envconfig:"USER" required:"true"`
		Password string `envconfig:"PASSWORD" required:"true"`
		Name     string `envconfig:"NAME" required:"true"`
	}
)
