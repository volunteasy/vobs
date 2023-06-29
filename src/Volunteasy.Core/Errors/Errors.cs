using System.ComponentModel;

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
public class OrganizationNotFoundException : ApplicationException
{
    public OrganizationNotFoundException(
        string? message = "Ops! Não encontramos esta organização") : base(message) {}
    
    public override string? HelpLink { get; set; } = "404";
}

// This exception is meant to be used in cases where the membership is not found
// and when the user does not have authorization to access it, so we don't implicitly
// inform that there is a membership for a given user.
public class MembershipNotFoundException : ApplicationException
{
    public MembershipNotFoundException(
        string? message = "Ops! Não encontramos esta inscrição") : base(message) {}
    
    public override string? HelpLink { get; set; } = "404";
}


public class DuplicateMembershipException : ApplicationException
{
    public DuplicateMembershipException(
        string? message = "Ops! Este membro já faz parte da organização") : base(message) {}
    
    public override string? HelpLink { get; set; } = "400";
}

public class DuplicateOrganizationException : ApplicationException
{
    public DuplicateOrganizationException(
        string? message = "Ops! Uma organização com este documento já existe") : base(message) {}
    
    public override string? HelpLink { get; set; } = "400";
}


public class InvalidMembershipFilterException : ApplicationException
{
    public InvalidMembershipFilterException(
        string? message = "Ops! Você precisa filtrar por uma organização ou usuário") : base(message) {}
    
    public override string? HelpLink { get; set; } = "400";
}

public class BenefitNotFoundException : ApplicationException
{
    public BenefitNotFoundException(
        string? message = "O benefício solicitado não foi encontrado") : base(message) {}
    
    public override string? HelpLink { get; set; } = "404";
}


public class ResourceNotFoundException : ApplicationException
{
    public ResourceNotFoundException(string message = "Ops! O recurso solicitado não foi encontrado")
    {
        Message = message;
    }

    public ResourceNotFoundException(Type resource)
    {
        var attribute = (DisplayNameAttribute?) Attribute.GetCustomAttribute(resource, typeof(DisplayNameAttribute));

        Message = attribute != null
            ? $"Ops, não foi possível encontrar um(a) {attribute.DisplayName.ToLower()} com os parâmetros informados"
            : "Ops! O recurso solicitado não foi encontrado";
    }

    public override string Message { get; }
}