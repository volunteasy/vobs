using System.Net.Http.Json;
using System.Security.Claims;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Volunteasy.Application;
using Volunteasy.Core.Data;
using Volunteasy.Core.DTOs;
using Volunteasy.Core.Errors;
using Volunteasy.Core.Model;

namespace Volunteasy.Infrastructure.Firebase;

public class Auth : IAuthenticator
{
    private readonly FirebaseAuth _auth;
    
    private readonly ILogger<Auth> _log;

    private readonly HttpClient _http;

    private readonly Data _data;

    private readonly string _signUpUrl;

    private readonly string _signInUrl;

    public Auth(FirebaseAuth auth, ILogger<Auth> log, Data data, string signInUrl, string signUpUrl)
    {
        _auth = auth;
        _log = log;
        _http = new HttpClient();
        _data = data;
        _signInUrl = signInUrl;
        _signUpUrl = signUpUrl;
    }

    public async Task<(string, string)> SignUp(UserCredentials identification)
    {
        var res = await _http.PostAsync(_signUpUrl, JsonContent.Create(new
        {
            identification.Email,
            identification.Password,
            ReturnSecureToken = true
        }));

        var body = await res.Content.ReadFromJsonAsync<FirebaseAuthResponse>();

        if (body?.Error is null)
            return (body?.LocalId ?? "", body?.IdToken ?? "");

        switch (body.Error.Message)
        {
            case "EMAIL_EXISTS":
                throw new DuplicateEmailException();
            default:
                _log.LogWarning("Unhandled firebase error: {Error}", body.Error.Message);
                throw new ApplicationException(body.Error.Message);
        }
    }

    public async Task<(string, string)> SignIn(UserCredentials identification)
    {
        var res = await _http.PostAsync(_signInUrl, JsonContent.Create(new
        {
            identification.Email,
            identification.Password,
            ReturnSecureToken = true
        }));

        var body = await res.Content.ReadFromJsonAsync<FirebaseAuthResponse>();

        if (body?.Error is null)
            return (body?.LocalId ?? "", body?.IdToken ?? "");

        switch (body.Error.Message)
        {
            case "INVALID_PASSWORD":
                throw new InvalidPasswordException();
            case "EMAIL_NOT_FOUND":
                throw new EmailNotFoundException();
            case "USER_DISABLED":
                throw new EmailNotFoundException();
            default:
                _log.LogWarning("Unhandled firebase error: {Error}", body.Error.Message);
                throw new ApplicationException(body.Error.Message);
        }
    }

    public async Task<ClaimsPrincipal?> GetClaimsByToken(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return null;
            
            var claims = await _auth.VerifyIdTokenAsync(token);
        
            if (claims is null || claims.ExpirationTimeSeconds <= 0) 
            {
                _log.LogWarning("firebase token verification returned null");
                return null;
            }
            
            var user = await _data.Users.SingleAsync(x => x.ExternalId == claims.Uid);

            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new(ClaimTypes.Expired, (claims.ExpirationTimeSeconds <= 0).ToString()),
                        new (ClaimTypes.UserData, claims.Uid),
                    }, "Bearer"
                )
            );
        }
        catch (Exception e)
        {
            _log.LogWarning("Could not get/parse token: {Exception}", e);
            return null;
        }
    }
}