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

public class BenefitItemAlreadySetException : ApplicationException
{
    public BenefitItemAlreadySetException(
        string? message = "Cada item deve ser único em um benefício") 
        : base(message) {}
    
    public override string? HelpLink { get; set; } = "412";
}


public class DistributionClosedOrFullException : ApplicationException
{
    public DistributionClosedOrFullException(
        string? message = "Esta distribuição já ocorreu ou não está aceitando novos beneficiários no momento") 
        : base(message) {}
    
    public override string? HelpLink { get; set; } = "412";
}