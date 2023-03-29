package mysql

import (
	"context"
	"database/sql"
	"fmt"
	"testing"

	"govobs/app/config"
	conn "govobs/app/providers/sql"
)

type DatabaseContextBuilder func(t *testing.T) *sql.DB

type Callback = func(ctx context.Context, t *testing.T)

func builder(c *sql.DB, port string) DatabaseContextBuilder {
	return func(t *testing.T) *sql.DB {
		t.Helper()

		test, name := t.Name(), createName()

		err := createDatabase(context.Background(), c, name)
		if err != nil {
			t.Errorf("Could not create database for test %s: %v", test, err)
			t.FailNow()
		}

		db, mig, err := conn.NewConnection(config.MySQL{
			DSN: fmt.Sprintf("root:volunteasy@tcp(localhost:%s)/%s?multiStatements=true", port, name),
		})
		if err != nil {
			t.Errorf("Could not connect to database for test %s: %v", test, err)
			t.FailNow()
		}

		err = mig()
		if err != nil {
			t.Errorf("Could not migrate database for test %s: %v", test, err)
			t.FailNow()
		}

		err = initializeData(db)
		if err != nil {
			t.Errorf("Could not add test data for test %s: %v", test, err)
			t.FailNow()
		}

		t.Cleanup(func() {
			db.Close()
		})

		return db
	}
}

func (b DatabaseContextBuilder) TX(t *testing.T) *sql.Tx {
	t.Helper()

	tx, _ := b(t).Begin()
	return tx
}

func initializeData(db *sql.DB) error {
	scripts := []string{
		`
			INSERT INTO organizations (id, name, document, phone, address)
			VALUES
				(1, 'Helping Hands', '12345678900', '5511999999999', '{"zipCode":"01001-000","houseNumber":"123","streetName":"Rua da Ajuda","complement":"Sala 1001","district":"Centro","city":"São Paulo","state":"SP","country":"Brasil"}'),
				(2, 'Volunteer Corps', '98765432100', '5511888888888', '{"zipCode":"02002-000","houseNumber":"456","streetName":"Rua do Voluntariado","complement":"Andar 5","district":"Pinheiros","city":"São Paulo","state":"SP","country":"Brasil"}'),
				(3, 'People United', '45678901200', '5511777777777', '{"zipCode":"03003-000","houseNumber":"789","streetName":"Rua da União","complement":"Loja 3","district":"Vila Mariana","city":"São Paulo","state":"SP","country":"Brasil"}'),
				(4, 'Community Outreach', '78901234500', '5511666666666', '{"zipCode":"04004-000","houseNumber":"2468","streetName":"Avenida da Comunidade","complement":"Sala 100","district":"Moema","city":"São Paulo","state":"SP","country":"Brasil"}'),
				(5, 'Neighbors Helping Neighbors', '23456789000', '5511555555555', '{"zipCode":"05005-000","houseNumber":"1357","streetName":"Rua dos Vizinhos","complement":"Apartamento 50","district":"Vila Leopoldina","city":"São Paulo","state":"SP","country":"Brasil"}');

		`,
		`
			INSERT INTO distributions (id, org_id, name, description, date, items, benefits_allowed, phone, address)
			VALUES 
				(1, 1, 'Food Distribution', 'Distributing food to those in need', '2023-03-28 10:00:00', 50, 2, '1234567890', '{"zipCode":"03939111","houseNumber":"77","streetName":"Rua da Casa","complement":"Xeks","district":"Almados","city":"Xiexie","state":"Xie","country":"Xi"}'),
				(2, 1, 'Clothes Donation', 'Collecting and donating clothes to those in need', '2023-03-30 14:00:00', 100, 1, '', '{}'),
				(3, 2, 'Soup Kitchen', 'Serving meals to the homeless', '2023-04-01 12:00:00', 75, 3, '', '{}'),
				(4, 2, 'Homeless Outreach', 'Providing basic necessities to the homeless', '2023-04-05 09:00:00', 30, 1, '', '{}'),
				(5, 3, 'Elderly Care', 'Visiting and assisting the elderly', '2023-04-10 10:00:00', 20, 2, '', '{}');
		`,
		`
			INSERT INTO memberships (user_id, org_id, role, status)
			VALUES 
				('c5c0e5b5-0b5a-44ec-9d37-f43b9f16a072', 1, 'owner', 'accepted'),
				('a98e7bd2-d9e9-4d60-9a9f-c5e0db10d7fa', 1, 'assisted', 'accepted'),
				('c5c0e5b5-0b5a-44ec-9d37-f43b9f16a072', 2, 'assisted', 'accepted'),
				('d9e6b929-f6f8-4b10-a6d1-6f7e78546d15', 2, 'assisted', 'accepted'),
				('38463f8b-cd9c-4d80-89a5-f5ce5dc5f5db', 2, 'assisted', 'accepted'),
				('2f91d4a7-758a-4f7c-b9ba-63011e17bb4d', 2, 'assisted', 'accepted'),
				('4a4a4dc3-3f31-4c60-81c3-0051655a9c5f', 2, 'volunteer', 'pending'),
				('a98e7bd2-d9e9-4d60-9a9f-c5e0db10d7fa', 3, 'volunteer', 'pending'),
				('d9e6b929-f6f8-4b10-a6d1-6f7e78546d15', 3, 'volunteer', 'pending');
		`,
		`
			INSERT INTO benefits (id, assisted_id, distribution_id, queue_position_id, claimed_at)
			VALUES 
				(1, 'd9e6b929-f6f8-4b10-a6d1-6f7e78546d15', 2, 1234567890, '2022-03-15 10:30:00'),
				(2, '38463f8b-cd9c-4d80-89a5-f5ce5dc5f5db', 2, 1234567891, '2022-03-17 13:45:00'),
				(3, '2f91d4a7-758a-4f7c-b9ba-63011e17bb4d', 2, 1234567892, '2022-03-17 13:50:00');
		`,
	}

	for _, script := range scripts {
		_, err := db.Exec(script)
		if err != nil {
			return err
		}
	}

	return nil
}
