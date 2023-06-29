using System.Net.Http.Json;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using Volunteasy.Application;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;

namespace Volunteasy.Infrastructure.Firebase;

public class Auth : IAuthenticator
{
    private readonly FirebaseAuth _auth;
    
    private readonly ILogger<Auth> _log;

    private readonly HttpClient _http;

    private readonly string _signInUrl;

    public Auth(FirebaseAuth auth, ILogger<Auth> log, string signInUrl)
    {
        _auth = auth;
        _log = log;
        _http = new HttpClient();
        _signInUrl = signInUrl;
    }

    public async Task<string> SignUp(long userId, UserCredentials identification)
    {
        try
        {
            var rec  = await _auth.CreateUserAsync(new UserRecordArgs
            {
                Email = identification.Email,
                Password = identification.Password,
            });

            await _auth.SetCustomUserClaimsAsync(rec.Uid, new Dictionary<string, object>
            {
                { "volunteasy_id", userId }
            });

            return rec.Uid;
        }
        catch (FirebaseAuthException e)
        {
            if (e.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
                throw new DuplicateUserException();

            _log.LogWarning(e, "Unhandled firebase error");
            throw;
        }
    }

    public async Task<string> SignIn(UserCredentials identification)
    {
        var res = await _http.PostAsync(_signInUrl, JsonContent.Create(new
        {
            identification.Email,
            identification.Password,
            ReturnSecureToken = true
        }));

        var body = await res.Content.ReadFromJsonAsync<FirebaseAuthResponse>();

        if (body?.Error is null)
            return body?.IdToken ?? "";

        switch (body.Error.Message)
        {
            case "INVALID_PASSWORD":
                throw new InvalidPasswordException();
            case "EMAIL_NOT_FOUND":
                throw new UserNotFoundException();
            case "USER_DISABLED":
                throw new UserNotFoundException();
            default:
                _log.LogWarning("Unhandled firebase error: {Error}", body.Error.Message);
                throw new ApplicationException($"Firebase: {body.Error.Message}");
        }
    }

    public Task RemoveUserByExternalId(string ext)
    {
        return _auth.DeleteUserAsync(ext);
    }
}