package main

import "net/http"

func main() {

	http.HandleFunc("/health", func(writer http.ResponseWriter, request *http.Request) {
		println("ni hao!")
    })

    err := http.ListenAndServe(":8080", nil)
    if err != nil {
        panic(err)
    }
}