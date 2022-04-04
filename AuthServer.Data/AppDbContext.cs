using AuthServer.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data
{

    // Identity üyelik tablolar
    public class AppDbContext:IdentityDbContext<UserApp,IdentityRole,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        // bu tabloların sütunları ne olacak gibi ayarları belirlemek için aşağıdaki gibi belirlenmesi gerekmektedir.

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);  
        }

    }

}
