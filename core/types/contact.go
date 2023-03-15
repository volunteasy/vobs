package types

type Contact struct {
	Address Address
	Phone   Phone
}


func (c Contact) Validate() error {
	if err := c.Address.Validate(); err != nil {
		return err
	}

	if err := c.Phone.Validate(); err != nil {
		return err
	}
	
	return nil
}