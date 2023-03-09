package types

import "strings"

type Nickname string

const NoNickname Nickname = ""

func (n Nickname) Validate() error {
	return nil
}

func (n Nickname) Normalize() Nickname {
	return Nickname(strings.ToLower(string(n)))
}
