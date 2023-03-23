FROM golang:1.20

WORKDIR /usr/src/govobs

# Copy the whole project into the container govobs folder
COPY . .
RUN go mod download && go mod verify



RUN go build -v -o /usr/local/bin/govobs ./cmd/api/main.go
RUN go build -v -o /usr/local/bin/govobs-cli ./cmd/cli/main.go

EXPOSE 8080

CMD ["govobs"]

