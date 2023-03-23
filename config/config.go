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
		Host     string `envconfig:"HOST"`
		User     string `envconfig:"USER"`
		Password string `envconfig:"PASSWORD"`
		Name     string `envconfig:"NAME"`
		DSN      string `envconfig:"DSN"`
		TLS      bool   `envconfig:"TLS"`
	}
)
