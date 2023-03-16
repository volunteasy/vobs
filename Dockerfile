FROM golang:alpine AS builder

RUN apk add --no-cache git

WORKDIR /go/src/app

COPY . .

RUN go mod tidy
RUN go build -o /go/bin/govobs -v ./cmd/api/main.go



FROM alpine:latest

RUN apk --no-cache add ca-certificates

COPY --from=builder /go/bin/govobs /app

ENTRYPOINT /app

LABEL Name=govobs Version=0.0.1

EXPOSE 8080