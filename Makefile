build: deps
	@echo "Building application"
	@go build ./...

deps:
	@echo "Installing dependencies"
	@go mod tidy
	@go install github.com/matryer/moq@v0.3.1
	@go install github.com/swaggo/swag/cmd/swag@v1.8.10


clean:
	@echo "Removing generated mock files"
	@find . -type f \( -name '*_mock.go' -o -name '*_mock_test.go' \) -exec rm {} +
	@rm -rf gen/*


gen: deps clean
	@echo "Generating new mock files"
	@go generate ./...
	@echo "Generating new swagger documentation files"
	@swag init -q -g ./cmd/api/main.go -o ./docs/swagger

test: gen
	@echo "Testing application"
	@go test ./... -v 