package query

import (
	"fmt"
	"strings"

	"govobs/app/core/types"
)

type Query struct {
	Args     []interface{}
	query    string
	operator string
}

func NewQuery(q string) Query {
	return Query{query: q}
}

func (q Query) Query() string {
	return q.query
}

func (q Query) And() Query {
	if len(q.Args) != 0 {
		q.operator = "and "
	}

	return q
}

func (q Query) Or() Query {
	if len(q.Args) != 0 {
		q.operator = "or "
	}

	return q
}

func (q Query) Where(condition bool, input string, values ...interface{}) Query {
	if !condition {
		return q
	}

	isMultipleParams := strings.Contains(input, "%s")

	if !isMultipleParams && strings.Count(input, "?") != len(values) {
		return q
	}

	// This array is an array containing string representation of the
	// numbers to be added in the query as arguments
	//
	// This case stands for multiple injections, as in an IN() condition
	// The user must not add any $ to their input, only the standard % flag
	// The code produces a string with placeholders separated by commas
	// like '$1, $2, $3, $4, $5', being the number of placeholders equal the number
	// of values passed in
	placeholders := make([]string, len(values))

	for i, value := range values {
		q.Args = append(q.Args, value)

		if isMultipleParams {
			placeholders[i] = "?"
		}
	}

	// Add 'OR' or 'AND' to the end of the current query.
	// If none of these conjunctions are set, adds a 'WHERE' clause
	operator := "where "
	if q.operator != "" {
		operator = q.operator
	}

	query := operator + input

	if !isMultipleParams {
		return q.appendToQuery(query)
	}

	return q.appendToQuery(fmt.Sprintf(query, strings.Join(placeholders, ", ")))
}

func (q Query) Limit(limits types.ListRange) Query {
	q.Args = append(q.Args,
		limits.Start,
		limits.Limit(),
	)

	return q.appendToQuery("limit ?, ?")
}

func (q Query) appendToQuery(append string) Query {
	q.query = fmt.Sprintf("%s %s ", q.query, append)
	return q
}
