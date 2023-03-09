package types

type Document string

const NoDocument Document = ""

func (d Document) Validate() error {
	return nil
}

func (d Document) Normalize() string {
	return ""
}
