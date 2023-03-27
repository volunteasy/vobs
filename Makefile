define compile
	@echo "Building $1"
	@go build -o build/volunteasy-$1 govobs/cmd/$1
endef


build:
	@echo "Building application"
	@go mod tidy
	$(call compile,api)
	$(call compile,cli)

deps:
	@echo "Installing dependencies"
	@go install github.com/matryer/moq@v0.3.1
	@go install github.com/swaggo/swag/cmd/swag@v1.8.10


clean:
	@echo "Removing generated mock files"
	@find . -type f \( -name '*_mock.go' -o -name '*_mock_test.go' \) -exec rm {} +
	@rm -rf gen/*


fmt:
	@echo "Formatting and organizing imports"
	@go install github.com/daixiang0/gci@v0.6.3
	@gci write --skip-generated .
	@go install mvdan.cc/gofumpt@v0.3.1
	@gofumpt -w -extra .
	@go fmt ./...
	@go vet ./...
	@make metalint


gen: deps clean
	@echo "Generating new mock files"
	@go generate ./...
	@echo "Generating new swagger documentation files"
	@swag init -q -g ./cmd/api/main.go -o ./docs/swagger
	@swag fmt ./cmd/api/main.go

test: gen
	@echo "Testing application"
	@go install github.com/rakyll/gotest@latest
	gotest -race -failfast -timeout 5m -count=1 ./...

run:
	@echo "Running API application"
	@go run ./cmd/api/main.go

local:
	@echo "Setting up service dependencies locally"
	@docker-compose --file ./deploy/local/docker-compose.yaml up
