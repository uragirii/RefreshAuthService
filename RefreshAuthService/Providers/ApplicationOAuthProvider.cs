using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using RefreshAuthService.Context;
using RefreshAuthService.Entities;
using RefreshAuthService.Models;

namespace RefreshAuthService.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = "*";
            
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            AuthenticationProperties properties = CreateProperties(user.UserName, context.ClientId);
            AuthenticationTicket ticket = new AuthenticationTicket(identity, properties);
            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // First get the clientId and client secret from the form body
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            Client client = null;
            context.TryGetFormCredentials(out clientId, out clientSecret);

            if(context.ClientId == null)
            {
                // Return a error if client id is not provided;
                context.SetError("invalid_client_id", "Client ID cannot be null");
                return Task.FromResult<object>(null);
            }

            using(AuthRepository _repo = new AuthRepository())
            {
                client = _repo.FindClient(clientId);
            }

            if(client == null)
            {
                context.SetError("invalid_client_id", "Client ID must be registered before");
                return Task.FromResult<object>(null);
            }
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                context.SetError("invalid_client_secret", "Client Secret must not be null");
                return Task.FromResult<object>(null);
            }
            if(clientSecret != client.ClientSecret)
            {
                context.SetError("invalid_client_secret", "Client Secret is invalid");
                return Task.FromResult<object>(null);
            }

            // If all the checks failed, validate the client;
            context.Validated();

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName, string cliendId)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                {
                        "as:client_id", (cliendId == null) ? string.Empty : cliendId
                    },
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}