package query

import (
	"database/sql"
	"errors"
	"fmt"

	"github.com/go-sql-driver/mysql"
)

func HandleNotFoundErr(err, mapto error) error {
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

func HandleExecSingleResult(result sql.Result, errNoRows error) error {
	rows, err := result.RowsAffected()
	if err != nil || rows == 1 {
		return nil
	}

	if rows == 0 {
		return errNoRows
	}

	return fmt.Errorf("affected more rows than expected (got: %d - wanted: 1)", rows)
}
