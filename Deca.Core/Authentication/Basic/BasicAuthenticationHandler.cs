using Desa.Core.Authentication.Resorces;
using Desa.Core.Repositories;
using Desa.Core.Repositories.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Desa.Core.Authentication.Basic
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        protected UserRepository _userRepository;
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _userRepository = new UserRepository();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out StringValues value))
                return await Task.FromResult(AuthenticateResult.Fail("Missing Authorization Key"));

            var authorizationHeader = value.ToString();

            if(!authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                return await Task.FromResult(AuthenticateResult.Fail("Authorization header does not start with 'Basic'"));

            var authBase64Decoded = Encoding.UTF8.GetString(
                Convert.FromBase64String(
                    authorizationHeader.Replace("Basic ", "",
                    StringComparison.OrdinalIgnoreCase)
                )
            );

            var authSplit = authBase64Decoded.Split([':'], 2);

            if(authSplit.Length != 2)
                return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header format"));

            var user = await _userRepository.ObtemUserPorAppTokenEAppKeyFromSQL(authSplit[0], authSplit[1]);

            if(user == null)
                return await Task.FromResult(AuthenticateResult.Fail("User Not Found"));

            var client = new BasicAuthenticationClient
            {
                AuthenticationType = "Basic",
                IsAuthenticated = true
            };

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(client, [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(CustomClaimTypes.User, JsonSerializer.Serialize(user))
            ]));

            return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
        }
    }
}
