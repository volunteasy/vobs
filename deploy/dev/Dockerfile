FROM golang:1.19-alpine3.17 as builder

WORKDIR /usr/src/govobs

# Copy the whole project into the container govobs folder
COPY . .
RUN go build -v -o ./govobs ./cmd/api/main.go
RUN go build -v -o ./govobs-cli ./cmd/cli/main.go

FROM alpine:latest

RUN apk --no-cache add ca-certificates

COPY --from=builder /usr/src/govobs /usr/local/bin

ENTRYPOINT govobs

LABEL Name=govobs Version=0.0.1

EXPOSE 8080

