build:
	@echo "Building application"
	@go mod tidy
	@go build ./...

deps:
	@echo "Installing dependencies"
	@go install github.com/matryer/moq@v0.3.1
	@go install github.com/swaggo/swag/cmd/swag@v1.8.10


clean:
	@echo "Removing generated mock files"
	@find . -type f \( -name '*_mock.go' -o -name '*_mock_test.go' \) -exec rm {} +
	@rm -rf gen/*


gen: deps clean
	@echo "Generating new mock files"
	@go generate ./...
# 	@echo "Generating new swagger documentation files"
# 	@swag init -q -g ./cmd/api/main.go -o ./docs/swagger

test: gen
	@echo "Testing application"
	@go test ./... -v

run:
	@echo "Running API application"
	@go run ./cmd/api/main.go

local:
	@echo "Setting up service dependencies locally"
	@docker-compose --file ./deploy/local/docker-compose.yaml up -d
