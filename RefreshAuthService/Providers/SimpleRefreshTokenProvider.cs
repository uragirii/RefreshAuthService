using Microsoft.Owin.Security.Infrastructure;
using RefreshAuthService.Context;
using RefreshAuthService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RefreshAuthService.Providers
{
    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientId = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrEmpty(clientId))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");

            using(AuthRepository _repo = new AuthRepository())
            {
                var token = new RefreshToken
                {
                    Id = refreshTokenId,
                    Client = clientId,
                    User = context.Ticket.Identity.Name,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                };

                context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;
                token.ProtectedTicket = context.SerializeTicket();

                var result = await _repo.AddRefreshToken(token);

                if (result)
                {
                    context.SetToken(refreshTokenId);
                }
            }
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            string refreshTokenId = context.Token;

            using(AuthRepository _repo = new AuthRepository())
            {
                var oldToken = await _repo.FindRefreshToken(refreshTokenId);
                if (oldToken != null)
                {
                    // Get the "Protected String" it will auto generate the new access token from it
                    context.DeserializeTicket(oldToken.ProtectedTicket);
                    var result = await _repo.RemoveRefreshToken(refreshTokenId);
                    // Remove the old token as now a new Token will be generated using the CreateAsync method
                }
                // If token is not found then token is invalid
                // Hence don't do anything request will fail

            }
        }
    }
}