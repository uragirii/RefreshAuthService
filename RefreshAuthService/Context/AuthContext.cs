using Microsoft.AspNet.Identity.EntityFramework;
using RefreshAuthService.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RefreshAuthService.Context
{
    // This class will be used to interface with Database
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext() : base("RefreshAuthService")
        {

        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}