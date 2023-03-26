package query

import (
	"fmt"
	"govobs/app/core/types"
	"strings"
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

	if strings.Count(q.query, "$") != len(values) {
		return q
	}

	// This array is an array containing string representation of the
	// numbers to be added in the query as arguments
	//
	// Ex $1, $2, $3
	placeholders := make([]any, len(values))

	// Add the values to be queried to the arguments array and set
	// numbers of placeholders based on the new arguments array length
	for i, value := range values {
		q.Args = append(q.Args, value)

		// The placeholder for the given value is always equal the length of
		// the current arguments list
		placeholders[i] = len(q.Args)
	}

	// Add 'OR' or 'AND' to the end of the current query.
	// If none of these conjunctions are set, adds a 'WHERE' clause
	operator := "where "
	if q.operator != "" {
		operator = q.operator
	}

	query := operator + input

	if strings.Contains(query, "$") {
		return q.appendToQuery(fmt.Sprintf(query, placeholders...))
	}

	// This case stands for multiple injections, as in an IN() condition
	// The user must not add any $ to their input, only the standard % flag
	// The code produces a string with placeholders separated by commas
	// like '$1, $2, $3, $4, $5', being the number of placeholders equal the number
	// of values passed in
	placeholdersStr := make([]string, len(values))

	// For each of the values in the placeholder array, add a $ to the beginning
	for i, value := range placeholders {
		placeholdersStr[i] = fmt.Sprintf("$%d", value)
	}

	return q.appendToQuery(fmt.Sprintf(query, strings.Join(placeholdersStr, ", ")))
}

func (q Query) Limit(limits types.ListRange) Query {
	q.Args = append(q.Args,
		limits.Limit()+1,
		limits.Start,
	)

	return q.appendToQuery(
		fmt.Sprintf("limit $%d, $%d",
			len(q.Args)-1,
			len(q.Args),
		),
	)

}

func (q Query) appendToQuery(append string) Query {
	q.query = fmt.Sprintf("%s %s ", q.query, append)
	return q
}
