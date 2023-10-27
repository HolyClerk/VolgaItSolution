using Simbir.Application.Other;
using Simbir.Core.Entities;
using Simbir.Core.Requests;
using Simbir.Core.Results;

namespace Simbir.Application.Services;

public class TransportService
{
    private readonly IDbContext _context;

    public TransportService(IDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Transport>> GetTransportAsync(int id)
    {
        var transport = await _context.Transports.FindAsync(id);

        if (transport is null)
            return Result<Transport>.Failed("Транспорт с id \"{id}\" не найден.");

        return Result<Transport>.Success(transport);
    }

    public async Task CreateTransportAsync(CreateTransportRequest request)
    {
        await _context.Transports.AddAsync(new Transport
        {

        });

        await _context.SaveChangesAsync();
    }

    public async Task UpdateTransportAsync(int id, UpdateTransportRequest request)
    {
        // только владелец
    }

    public async Task RemoveTransportAsync(int id)
    {
        // только владелец

    }
}
