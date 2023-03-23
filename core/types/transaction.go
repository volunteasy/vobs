package types

import "context"

type (
	TransactionOpener func(ctx context.Context) (context.Context, TransactionCloser)

	TransactionCloser func(ctx context.Context)

	TransactionContextKey struct{}
)
