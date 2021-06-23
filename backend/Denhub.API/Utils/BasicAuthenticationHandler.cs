using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Denhub.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Denhub.API.Utils {
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> {
        private readonly IOptions<SecurityOptions> _securityOptions;

        public BasicAuthenticationHandler(IOptions<SecurityOptions> securityOptions, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) {
            _securityOptions = securityOptions;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null) {
                return AuthenticateResult.NoResult();
            }

            if (!Request.Headers.ContainsKey("Authorization")) {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            string username;
            string password;

            try {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] {':'}, 2);
                username = credentials[0];
                password = credentials[1];
            }
            catch {
                return AuthenticateResult.Fail("Authorization Header is malformed or missing");
            }

            if (_securityOptions.Value.BasicAuth.Username == username &&
                _securityOptions.Value.BasicAuth.Password == password) {
                var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Name, username)
                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            return AuthenticateResult.Fail("Invalid username or password");
        }
    }
}