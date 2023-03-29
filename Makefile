define compile
	@echo "Building $1"
	@go build -o build/volunteasy-$1 govobs/cmd/$1
endef


compile:
	@echo "Building application"
	@go mod tidy
	@go build ./...
	$(call compile,api)
	$(call compile,cli)

clean:
	@echo "Removing generated mock files"
	@find . -type f \( -name '*_mock.go' -o -name '*_mock_test.go' \) -exec rm {} +
	@rm -rf gen/*


fmt:
	@echo "Formatting and organizing imports"
	@go install github.com/daixiang0/gci@v0.6.3
	@go install github.com/swaggo/swag/cmd/swag@v1.8.10
	@swag fmt ./cmd/api/main.go
	@gci write --skip-generated .
	@go install mvdan.cc/gofumpt@v0.3.1
	@gofumpt -w -extra .
	@go fmt ./...
	@go vet ./...

lint:
	@echo "Installing golangci-lint"
	@curl -sSfL https://raw.githubusercontent.com/golangci/golangci-lint/master/install.sh | sh -s -- -b $$(go env GOPATH)/bin v1.52.2
	@echo "Running linter"
	@$$(go env GOPATH)/bin/golangci-lint run --allow-parallel-runners --fix
	@$$(go env GOPATH)/bin/golangci-lint run --allow-parallel-runners -c ./.golangci_not_auto.yml


gen: clean
	@echo "Generating new mock files"
	@go install github.com/matryer/moq@v0.3.1
	@go install github.com/swaggo/swag/cmd/swag@v1.8.10
	@go generate ./...
	@echo "Generating new swagger documentation files"
	@swag init -q -g ./cmd/api/main.go -o ./docs/swagger

test: gen
	@echo "Testing application"
	@go install github.com/rakyll/gotest@latest
	gotest -race -failfast -timeout 5m -count=1 -coverprofile=coverage.out ./... 
	@go tool cover -html=coverage.out -o coverage.html

run:
	@echo "Running API application"
	@go run ./cmd/api/main.go

local:
	@echo "Setting up service dependencies locally"
	@docker-compose --file ./deploy/local/docker-compose.yaml up
