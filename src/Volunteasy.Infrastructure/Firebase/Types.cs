namespace Volunteasy.Infrastructure.Firebase;

public record FirebaseAuthResponse {
    public string? IdToken { get; init; }
    public string? LocalId { get; init; }
    public Error? Error { get; init; }
}

public record Error
{
    public int Code { get; init; }
    public string? Message { get; init; }
}