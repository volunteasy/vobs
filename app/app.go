package app

import (
	"database/sql"
	"govobs/app/config"
	"govobs/app/core/types"
	orgactions "govobs/app/providers/sql/actions/organization"

	"govobs/app/obs"

	"govobs/app/providers/snowflakeid"

	"github.com/aws/aws-sdk-go/service/cognitoidentityprovider"
	"github.com/bwmarrin/snowflake"
	"github.com/sirupsen/logrus"

	userjobs "govobs/app/jobs/user"

	useractions "govobs/app/providers/identity/user"
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

	userActions := useractions.NewActions(config.AWS.UserPoolID, deps.Cognito)

	orgActions := orgactions.NewActions(deps.DB)

	userJobs := userjobs.NewJobs(userActions, nil, nil, orgActions, ids)

	app := App{
		Users:  userJobs,
		Logger: deps.Logger,
		IDs:    ids,
	}

	return app, nil

}
