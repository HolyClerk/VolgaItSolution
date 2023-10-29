using Microsoft.EntityFrameworkCore;
using Simbir.Core.Entities;

namespace Simbir.Application.Other;

public interface IDbContext
{
    DbSet<Administrator> Administrators { get; }
    DbSet<ApplicationUser> Users { get; }
    DbSet<Transport> Transports { get; }
    DbSet<Rent> Rents { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}