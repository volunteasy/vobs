package docs

import (
	"net/http"

	_ "govobs/docs/swagger"

	httpSwagger "github.com/swaggo/http-swagger"
)

const redocTemplate = `
<!DOCTYPE html>
<html lang="en">
<head>
    <title>Govobs - Redoc</title>
    <!-- needed for adaptive design -->
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="https://fonts.googleapis.com/css?family=Montserrat:300,400,700|Roboto:300,400,700" rel="stylesheet">

    <!--
    Redoc doesn't change outer page styles
    -->
    <style>
        body {
            margin: 0;
            padding: 0;
        }
    </style>
</head>
<body>
<redoc spec-url="/docs/swagger/doc.json"></redoc>
<script src="https://cdn.redoc.ly/redoc/latest/bundles/redoc.standalone.js"></script>
</body>
</html>

`

func Redoc() http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Content-Type", "text/html")
		w.Write([]byte(redocTemplate))
	}
}

func Swagger() http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		httpSwagger.WrapHandler.ServeHTTP(w, r)
	}
}
