package types

import (
	"fmt"
	"strconv"
	"strings"
)

const ZeroID ID = 0

const ZeroUserID = ""

type ID int64

type UserID string

type IDCreator = func() ID

func (i ID) String() string {
	return strconv.FormatInt(int64(i), 10)
}

func (i ID) MarshalJSON() ([]byte, error) {
	return []byte(fmt.Sprintf(`"%s"`, i.String())), nil
}

func (i *ID) UnmarshalJSON(b []byte) error {
	id, err := FromString(strings.ReplaceAll(string(b), `"`, ""))
	if err != nil {
		return err
	}

	*i = id

	return nil
}

var ErrInvalidID = ErrInvalidField("O ID envidado não pôde ser lido pois é inválido")

func FromString(in string) (ID, error) {
	i, err := strconv.Atoi(in)
	if err != nil {
		return 0, ErrInvalidID
	}

	return ID(i), nil
}