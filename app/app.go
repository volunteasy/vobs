package app

import (
	"database/sql"
	"govobs/config"

	"govobs/obs"

	"govobs/providers/snowflakeid"

	"github.com/aws/aws-sdk-go/service/cognitoidentityprovider"
	"github.com/bwmarrin/snowflake"
	"github.com/sirupsen/logrus"

	userjobs "govobs/jobs/user"

	useractions "govobs/providers/identity/user"
)

type Deps struct {
	DB      *sql.DB
	Cognito *cognitoidentityprovider.CognitoIdentityProvider
	Logger  *logrus.Entry
	IDNode  *snowflake.Node
}

type App struct {
	Users userjobs.Jobs

	Logger *logrus.Entry
}

func NewApp(deps Deps, config config.Config) (App, error) {

	obs.NewLogger(deps.Logger)

	ids := snowflakeid.NewIDCreator(deps.IDNode)

	userActions := useractions.NewActions(config.AWS.UserPoolID, deps.Cognito)

	userJobs := userjobs.NewJobs(userActions, nil, nil, nil, ids)

	app := App{
		Users:  userJobs,
		Logger: deps.Logger,
	}

	return app, nil

}
