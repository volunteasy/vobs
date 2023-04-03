package conn

import (
	"context"
	"database/sql"
	"embed"
	"errors"
	"fmt"
	"net/http"

	"github.com/golang-migrate/migrate/v4"
	"github.com/golang-migrate/migrate/v4/database/mysql"
	"github.com/golang-migrate/migrate/v4/source/httpfs"
)

func MigrationHandler(db *sql.DB) (*migrate.Migrate, error) {
	ctx := context.Background()
	co, _ := db.Conn(ctx)

	source, err := httpfs.New(http.FS(migrationsFS), "migrations")
	if err != nil {
		return nil, fmt.Errorf("failed to create migration httpfs driver: %w", err)
	}

	d, err := mysql.WithConnection(ctx, co, &mysql.Config{})
	if err != nil {
		return nil, fmt.Errorf("failed to create mysql migration instance: %w", err)
	}

	migration, err := migrate.NewWithInstance("httpfs", source, "", d)
	if err != nil {
		return nil, fmt.Errorf("failed to create migration source instance: %w", err)
	}

	return migration, err
}

func MigrateDatabase(db *sql.DB, addTestData bool) error {
	migration, err := MigrationHandler(db)
	if err != nil {
		return err
	}

	defer migration.Close()

	if err := migration.Up(); err != nil && !errors.Is(err, migrate.ErrNoChange) {
		return fmt.Errorf("error executing migration: %w", err)
	}

	if addTestData {
		err = AddTestData(db)
	}

	return err
}

func AddTestData(db *sql.DB) error {
	_, err := db.Exec(script)
	return err
}

//go:embed migrations
var migrationsFS embed.FS

const script = `
	INSERT INTO organizations (id, name, document, phone, address)
	VALUES
		(1, 'Helping Hands', '12345678900', '5511999999999', '{"zipCode":"01001-000","houseNumber":"123","streetName":"Rua da Ajuda","complement":"Sala 1001","district":"Centro","city":"São Paulo","state":"SP","country":"Brasil"}'),
		(2, 'Volunteer Corps', '98765432100', '5511888888888', '{"zipCode":"02002-000","houseNumber":"456","streetName":"Rua do Voluntariado","complement":"Andar 5","district":"Pinheiros","city":"São Paulo","state":"SP","country":"Brasil"}'),
		(3, 'People United', '45678901200', '5511777777777', '{"zipCode":"03003-000","houseNumber":"789","streetName":"Rua da União","complement":"Loja 3","district":"Vila Mariana","city":"São Paulo","state":"SP","country":"Brasil"}'),
		(4, 'Community Outreach', '78901234500', '5511666666666', '{"zipCode":"04004-000","houseNumber":"2468","streetName":"Avenida da Comunidade","complement":"Sala 100","district":"Moema","city":"São Paulo","state":"SP","country":"Brasil"}'),
		(5, 'Neighbors Helping Neighbors', '23456789000', '5511555555555', '{"zipCode":"05005-000","houseNumber":"1357","streetName":"Rua dos Vizinhos","complement":"Apartamento 50","district":"Vila Leopoldina","city":"São Paulo","state":"SP","country":"Brasil"}');
		

	INSERT INTO distributions (id, org_id, name, description, date, items, benefits_allowed, phone, address)
	VALUES 
		(1, 1, 'Food Distribution', 'Distributing food to those in need', '2023-03-28 10:00:00', 50, 2, '1234567890', '{"zipCode":"03939111","houseNumber":"77","streetName":"Rua da Casa","complement":"Xeks","district":"Almados","city":"Xiexie","state":"Xie","country":"Xi"}'),
		(2, 1, 'Clothes Donation', 'Collecting and donating clothes to those in need', '2023-03-30 14:00:00', 100, 1, '', '{}'),
		(3, 2, 'Soup Kitchen', 'Serving meals to the homeless', '2023-04-01 12:00:00', 75, 3, '', '{}'),
		(4, 2, 'Homeless Outreach', 'Providing basic necessities to the homeless', '2023-04-05 09:00:00', 30, 1, '', '{}'),
		(5, 3, 'Elderly Care', 'Visiting and assisting the elderly', '2023-04-10 10:00:00', 20, 2, '', '{}');
		

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
		

	INSERT INTO benefits (id, assisted_id, distribution_id, queue_position_id, claimed_at)
	VALUES 
		(1, 'd9e6b929-f6f8-4b10-a6d1-6f7e78546d15', 2, 1234567890, '2022-03-15 10:30:00'),
		(2, '38463f8b-cd9c-4d80-89a5-f5ce5dc5f5db', 2, 1234567891, '2022-03-17 13:45:00'),
		(3, '2f91d4a7-758a-4f7c-b9ba-63011e17bb4d', 2, 1234567892, '2022-03-17 13:50:00');
`