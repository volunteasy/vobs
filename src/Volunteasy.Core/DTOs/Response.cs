using System.Net;

namespace Volunteasy.Core.DTOs;

public record Response<T>
{
    public T? Data { get; init; }

    public HttpStatusCode Status { get; init; }

    public string? Message { get; init; }

    public bool HasNext { get; init; }

    // Created is a builds a response for when a resource is successfully persisted in the database
    public static Response<T> Created(T data)
    {
        return new Response<T>
        {
            Data = data,
            Status = HttpStatusCode.Created,
            Message = "Criado com sucesso!"
        };
    }

    // List is a response wrapper for multiple records responses
    public static Response<T> List(T data, int count, bool hasNext)
    {
        return new Response<T>
        {
            Data = data,
            Status = HttpStatusCode.OK,
            Message = $"Retornando lista com {count} itens",
            HasNext = hasNext
        };
    }

    // Content is a reneric successful response with a piece of data. Useful for GetByID requests
    public static Response<T> Content(T data)
    {
        return new Response<T>
        {
            Data = data,
            Status = HttpStatusCode.OK,
            Message = $"Recurso encontrado com sucesso",
        };
    }

    // NoContent is used when a request is successful, but there's nothing valueable to be returned
    public static Response<T> NoContent(string message)
    {
        return new Response<T>
        {
            Status = HttpStatusCode.NoContent,
            Message = message,
        };
    }


    public static Response<T> Unauthorized(string message)
    {
        return new Response<T>
        {
            Status = HttpStatusCode.Unauthorized,
            Message = message,
        };
    }

    // NotFound is used when the resource was not found in the repos
    public static Response<T> NotFound(string message)
    {
        return new Response<T>
        {
            Status = HttpStatusCode.NotFound,
            Message = message,
        };
    }

    // BadRequest is used when something is broken in the request, but we are not sure exactly of what it is
    public static Response<T> BadRequest(string message)
    {
        return new Response<T>
        {
            Status = HttpStatusCode.BadRequest,
            Message = message
        };
    }

    // ConstraintError is used when a validation has failed and we want the client to know that explicitly
    public static Response<T> ConstraintError(string message)
    {
        return new Response<T>
        {
            Status = HttpStatusCode.PreconditionFailed,
            Message = message
        };
    }

    // UnhandledError is used when we get an error we were not expecting at all
    public static Response<T> UnhandledError(string message)
    {
        return new Response<T>
        {
            Status = HttpStatusCode.InternalServerError,
            Message = message,
        };
    }
}