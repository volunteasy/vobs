namespace Volunteasy.Core.Errors;

public class BenefitItemsCountException : ApplicationException
{
    public BenefitItemsCountException(
        string? message = "Escolha pelo menos um item para seu benefício") 
        : base(message) {}
    
    public override string? HelpLink { get; set; } = "400";
}

public class BenefitItemQuantityException : ApplicationException
{
    public BenefitItemQuantityException(
        string? message = "Os itens devem ter quantidade maior que 0") 
        : base(message) {}
    
    public override string? HelpLink { get; set; } = "400";
}

public class BenefitUnauthorizedForUserException : ApplicationException
{
    public BenefitUnauthorizedForUserException(
        string? message = "Este benefício não está disponível para você no momento. Tente novamente mais tarde") 
        : base(message) {}
    
    public override string? HelpLink { get; set; } = "400";
}