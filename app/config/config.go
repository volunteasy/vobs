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
		DSN         string `envconfig:"DSN"`
		AddTestData bool   `envconfig:"ADD_TEST_DATA"`
	}

	AWS struct {
		Region          string `envconfig:"REGION"`
		UserClientID    string `envconfig:"USER_CLIENT_ID"`
		UserPoolID      string `envconfig:"USER_POOL_ID"`
		AccessKeyID     string `envconfig:"ACCESS_KEY_ID"`
		AccessKeySecret string `envconfig:"ACCESS_KEY_SECRET"`
	}
)
