using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using UserApi.Context;

namespace UserApi.AuthHandlers;

public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly DataContext _dataContext;

    public BasicAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock, DataContext dataContext) : 
        base(options, logger, encoder, clock)
    {
        _dataContext = dataContext;
    }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers["Authorization"].ToString();
        if (authorizationHeader != null && authorizationHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
        {
            var token = authorizationHeader.Substring("Basic ".Length).Trim();
            var credentialsAsEncodedString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var credentials = credentialsAsEncodedString.Split(':');
            var user = _dataContext.Users.FirstOrDefault(u => u.Login == credentials[0] && u.Password == credentials[1] && u.RevokedOn == null);
            if (user != null)
            {
                var role = user.Admin ? "Admin" : "Regular";
                var claims = new[] { new Claim(ClaimTypes.Name, credentials[0]),new Claim(ClaimTypes.Role,role) };
                var identity = new ClaimsIdentity(claims, "Basic");
                var claimsPrincipal = new ClaimsPrincipal(identity);
                return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
            }
        }
        Response.StatusCode = 401;
        return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
    }
}