package query

import (
	"database/sql"
	"errors"
	"github.com/go-sql-driver/mysql"
)

func HandleNotFoundErr(err error, mapto error) error {
	if err == nil {
		return nil
	}

	if errors.Is(err, sql.ErrNoRows) {
		return mapto
	}

	return err
}

func HandleDatabaseError(err error, handle map[uint16]error) error {
	if err == nil {
		return nil
	}

	var mysqlErr *mysql.MySQLError
	if !errors.As(err, &mysqlErr) {
		return err
	}

	mappedErr, ok := handle[mysqlErr.Number]
	if !ok {
		return err
	}

	return mappedErr

}
