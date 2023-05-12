
ifndef env
    env := local
endif

.PHONY: migrate
migrate:
	@dotnet ef migrations add $(name) --project src/Web

.PHONY: up
up:
	@echo "Creating application containers in the $(env) environment"
	@docker-compose --file ./deploy/$(env)/docker-compose.yaml -p nrbs up --build -d

.PHONY: down
down: 
	@echo "Removing application containers in the $(env) environment"
	@docker-compose --file ./deploy/$(env)/docker-compose.yaml -p nrbs down

.PHONY: build
build:
	@dotnet build

.PHONY: test
test:
	@dotnet test --no-restore --verbosity normal