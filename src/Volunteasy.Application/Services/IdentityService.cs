using Volunteasy.Core.Data;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;
using Volunteasy.Core.Services;

namespace Volunteasy.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly Data _data;

    private readonly IAuthenticator _authenticator;

    public IdentityService(Data data, IAuthenticator authenticator)
    {
        _data = data;
        _authenticator = authenticator;
    }

    public async Task<Response<UserToken>> SignUp(UserIdentification identification)
    {
        try
        {
            var (externalId, token) = await _authenticator.SignUp(new UserCredentials
            {
                Email = identification.Email,
                Password = identification.Password
            });

            _data.Add(new User
            {
                ExternalId = externalId,
                Document = identification.Document,
                Name = identification.Name
            });

            await _data.SaveChangesAsync();
            return Response<UserToken>.Created(new UserToken
            {
                Token = token
            });
        }
        catch (DuplicateEmailException e)
        {
            return Response<UserToken>.BadRequest("Este e-mail já está sendo usado por outra conta");
        }
        catch (Exception e)
        {
            return Response<UserToken>.UnhandledError(e.Message);
        }
    }
    
    public async Task<Response<UserToken>> SignIn(UserCredentials identification)
    {
        try
        {
            var (_, token) = await _authenticator.SignIn(identification);
            return Response<UserToken>.Content(new UserToken { Token = token });
        }
        catch (InvalidPasswordException)
        {
            return Response<UserToken>.BadRequest("Este e-mail já está sendo usado por outra conta");
        }
        catch (EmailNotFoundException)
        {
            return Response<UserToken>.BadRequest("Este e-mail não foi encontrado para nenhum usuário");
        }
        catch (Exception e)
        {
            return Response<UserToken>.UnhandledError(e.Message);
        }
    }
}