using Volunteasy.Core.Model;

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

public class BeneficiaryHasRecentClaimException : ApplicationException
{
    
    public BeneficiaryHasRecentClaimException(Benefit b, int validTimeSpan)
    {
        if (b.ClaimedAt == null)
            throw new ArgumentException("ClaimedAt in BeneficiaryHasRecentClaimException was null");

        var wait = b.ClaimedAt!.Value.AddDays(validTimeSpan) - DateTime.UtcNow;
        
        Message = 
            $"Você recebeu um benefício em {b.ClaimedAt:dd/MM/yyyy} e deve esperar {wait.Days} dias para receber outro.";
    }

    public override string Message { get; }

    public override string? HelpLink { get; set; } = "412";
}

public class BeneficiaryHasOpenDistributionException : ApplicationException
{
    public BeneficiaryHasOpenDistributionException(Benefit b)
    {
        var distribution = b.Distribution;
        if (distribution == null)
            throw new ArgumentException("Distribution in BeneficiaryHasOpenClaimException was null");
        
        Message = 
            $"Você já está cadastrado em uma distribuição que ocorre no dia {distribution.StartsAt:dd/MM/yyyy}.";
    }

    public override string Message { get; }

    public override string? HelpLink { get; set; } = "412";
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