namespace Volunteasy.Core.Errors;

public class InvalidPasswordException : ApplicationException
{
    public InvalidPasswordException(string? message = "") : base(message) {}
    
    public override string? HelpLink { get; set; } = "400";
}

public class DuplicateUserException : ApplicationException
{
    public DuplicateUserException(string? message = "Ops! Parece que este usuário já existe") : base(message) {}
    
    public override string? HelpLink { get; set; } = "400";
}

public class UserNotFoundException : ApplicationException
{
    public UserNotFoundException(string? message = "") : base(message) {}
    
    public override string? HelpLink { get; set; } = "404";
}

public class UserNotAuthorizedException : ApplicationException
{
    public UserNotAuthorizedException(string? message = "Ops! Você não tem permissão para realizar esta ação") : base(message) {}
    
    public override string? HelpLink { get; set; } = "401";
}