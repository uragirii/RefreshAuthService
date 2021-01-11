using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RefreshAuthService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RefreshAuthService.Context
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;
        private UserManager<IdentityUser> _userManager;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        public Client FindClient(string clientId)
        {
            return _ctx.Clients.Find(clientId);
        } 

        public void AddClient(Client client)
        {
            _ctx.Clients.Add(client);
            _ctx.SaveChanges();
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            // Check if there is existing token for this user and client
            // If token is there -> Remove the token and if not Add the token
            var prevRefreshToken = _ctx.RefreshTokens.Where(r => r.Client == token.Client && r.User == token.User).SingleOrDefault();
            if(prevRefreshToken != null)
            {
                // Delete the pre-existing token
                var result = await RemoveRefreshToken(prevRefreshToken);
            }

            // After token is deleted or token is not there, 
            // Add the new token
            _ctx.RefreshTokens.Add(token);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _ctx.RefreshTokens.Remove(refreshToken);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var token = await _ctx.RefreshTokens.FindAsync(refreshTokenId);
            if(token!= null)
            {
                _ctx.RefreshTokens.Remove(token);
                return await _ctx.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            return await _ctx.RefreshTokens.FindAsync(refreshTokenId);
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _ctx.RefreshTokens.ToList();
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }
    }
}