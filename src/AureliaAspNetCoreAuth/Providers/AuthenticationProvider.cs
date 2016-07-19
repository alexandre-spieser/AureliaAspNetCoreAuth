using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Server;
using System;

namespace AureliaAspNetCoreAuth.Providers
{
    public class AuthenticationProvider : OpenIdConnectServerProvider
    {
        public override Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            if (context.ClientId == "AureliaNetAuthApp")
            {
                // Note: the context is marked as skipped instead of validated because the client
                // is not trusted (JavaScript applications cannot keep their credentials secret).
                context.Skip();
            }

            else {
                // If the client_id doesn't correspond to the
                // intended identifier, reject the request.
                context.Reject(OpenIdConnectConstants.Errors.InvalidClient);
            }

            return Task.FromResult(0);
        }

        public override Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            if (context.Request.IsPasswordGrantType())
            {
                // make db query here
                var user = new { Id = "users-123", Email = "alex@123.com", Password = "AureliaNetAuth" };

                if (!string.Equals(context.Request.Username, user.Email, StringComparison.Ordinal) ||
                    !string.Equals(context.Request.Password, user.Password, StringComparison.Ordinal))
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Invalid username or password.");

                    return Task.FromResult(0);
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationScheme);
                identity.AddClaim(ClaimTypes.NameIdentifier, user.Id, OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);
                identity.AddClaim(ClaimTypes.Name, user.Email, OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);

                context.Validate(new ClaimsPrincipal(identity));
            }
                

            return Task.FromResult(0);
        }
    }
}
