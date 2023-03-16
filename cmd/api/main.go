package main

import (
    "fmt"
    "net/http"
)

func main() {

	http.HandleFunc("/health", func(writer http.ResponseWriter, request *http.Request) {
		writer.Write([]byte("Ni hao!"))
    })

	fmt.Println("Listening to endpoints")
    err := http.ListenAndServe(":8080", nil)
    if err != nil {
        panic(err)
    }
}