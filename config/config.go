package config

type (
	Config struct {
		Name        string `envconfig:"APP_NAME" default:"govobs"`
		Environment string `envconfig:"ENVIRONMENT" default:"development"`

		API   API
		MySQL MySQL
		AWS   AWS
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

	AWS struct {
		Region          string `envconfig:"REGION"`
		UserPoolID      string `envconfig:"USER_POOL_ID"`
		AccessKeyID     string `envconfig:"ACCESS_KEY_ID"`
		AccessKeySecret string `envconfig:"ACCESS_KEY_SECRET"`
	}
)
