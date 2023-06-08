using System.Text.Json.Serialization;

namespace Volunteasy.Api.Response;

public record Response
{
    [JsonIgnore]
    public HttpContext Context { get; init; } = null!;

    public int Status => Context.Response.StatusCode;

    public bool Success => Context.Response.StatusCode is >= 200 and <= 299;
    
    public string CorrelationId => Context.TraceIdentifier;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Reason { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Data { get; init; }
}