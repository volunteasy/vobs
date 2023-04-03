package app

import (
	"database/sql"

	"govobs/app/config"
	"govobs/app/core/types"
	userjobs "govobs/app/jobs/user"
	"govobs/app/obs"
	useractions "govobs/app/providers/identity/user"
	membershipActions "govobs/app/providers/mysql/membership"
	orgactions "govobs/app/providers/mysql/organization"
	"govobs/app/providers/snowflakeid"

	"github.com/aws/aws-sdk-go/service/cognitoidentityprovider"
	"github.com/bwmarrin/snowflake"
	"github.com/sirupsen/logrus"
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
	IDs    types.IDCreator
}

func NewApp(deps Deps, config config.Config) (App, error) {
	obs.NewLogger(deps.Logger)

	ids := snowflakeid.NewIDCreator(deps.IDNode)

	userActions := useractions.NewActions(config.AWS.UserPoolID, config.AWS.UserClientID, deps.Cognito)

	orgActions := orgactions.NewActions(deps.DB)

	mebrActions := membershipActions.NewActions(deps.DB)

	userJobs := userjobs.NewJobs(userActions, nil, mebrActions, orgActions, ids)

	app := App{
		Users:  userJobs,
		Logger: deps.Logger,
		IDs:    ids,
	}

	return app, nil
}
