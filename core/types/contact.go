package types

type Contact struct {
	Address Address
	Phone   Phone
}


func (c Contact) Validate() error {
	return nil
}