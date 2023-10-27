using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Simbir.Application.Other;
using Simbir.Core.Entities;

namespace Simbir.Infrastructure.Context;

public class ApplicationContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>, IDbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
      : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<ApplicationUser> Users { get; }
}
