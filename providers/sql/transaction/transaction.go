package transaction

import (
	"context"
	"database/sql"
	"govobs/core/types"
)

func FromContext(ctx context.Context) *sql.Tx {
	tx, ok := ctx.Value(types.TransactionContextKey{}).(*sql.Tx)
	if !ok {
		panic("transaction was not previously set in context")
	}

	return tx
}

func NewTransactionOpener(db *sql.DB) types.TransactionOpener {
	return func(ctx context.Context) (context.Context, types.TransactionCloser) {
		var tx *sql.Tx

		for tx == nil {
			tx, _ = db.BeginTx(ctx, nil)
		}

		return context.WithValue(ctx, types.TransactionContextKey{}, tx), func(ctx context.Context) {
			tx := FromContext(ctx)

			if err := tx.Commit(); err == nil {
				return
			}

			if err := tx.Rollback(); err != nil {
				return // TODO: add log saying that it could not commit nor close the transaction
			}
		}

	}
}
