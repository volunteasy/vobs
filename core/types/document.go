package types

import (
	"errors"
	"regexp"
	"strings"
)

type Document string

const NoDocument Document = ""

var (
	rgRegexp   = regexp.MustCompile(`^\d{2}\.\d{3}\.\d{3}-\d$`)
	cpfRegexp  = regexp.MustCompile(`^\d{3}\.\d{3}\.\d{3}-\d{2}$`)
	cnpjRegexp = regexp.MustCompile(`^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$`)

	ErrInvalidDocument = errors.New("invalid document")
)

func (d Document) Validate() error {
	doc := string(d)

	if cpfRegexp.MatchString(doc) || rgRegexp.MatchString(doc) || cnpjRegexp.MatchString(doc){
		return ErrInvalidDocument
	}

	return nil
}

func (d Document) Normalize() Document {
	doc := strings.ReplaceAll(string(d), " ", "")
	doc = strings.ReplaceAll(doc, ".", "")
	doc = strings.ReplaceAll(doc, "-", "")
	doc = strings.ReplaceAll(doc, "/", "")

	return Document(doc)
}