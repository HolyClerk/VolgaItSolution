using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Simbir.Application.Abstractions;
using Simbir.Application.Other;
using Simbir.Core.Entities;
using Simbir.Core.Requests;
using Simbir.Core.Results;

namespace Simbir.Application.Services;

public class TransportService : ITransportService
{
    private readonly IDbContext _context;
    private readonly IAccountService _accountService;

    public TransportService(IDbContext context, 
        IAccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    public async Task<List<Transport>> GetInRange(int start, int count)
    {
        return await _context.Transports
            .Where(t => t.Id >= start && t.Id <= count)
            .ToListAsync();
    }

    public async Task<Result<Transport>> GetAsync(long id)
    {
        var transport = await _context.Transports.FindAsync(id);

        if (transport is null)
            return Result<Transport>.Failed($"Транспорт с id \"{id}\" не найден.");

        return Result<Transport>.Success(transport);
    }

    public async Task<Result> AddAsync(AddTransportRequest request, ClaimsPrincipal userClaims)
    {
        var user = await _accountService.GetUserByClaimsAsync(userClaims);

        if (user is null)
            return Result.Failed();

        var newTransport = CreateTransport(user.Id, request);
        await _context.Transports.AddAsync(newTransport);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> AddAsync(ForceAddTransportRequest request)
    {
        var user = await _accountService.GetUserByIdAsync(request.OwnerId);

        if (user is null)
            return Result.Failed();

        var newTransport = CreateTransport(user.Id, request);
        await _context.Transports.AddAsync(newTransport);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(long transportId, UpdateTransportRequest request,
        ClaimsPrincipal userClaims)
    {
        var user = await _accountService.GetUserByClaimsAsync(userClaims);
        var transport = await _context.Transports.FindAsync(transportId);

        if (user is null || transport is null || transport.OwnerId != user.Id)
            return Result.Failed();

        UpdateTransport(transport, request);
        _context.Transports.Update(transport);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> ForceUpdateAsync(long transportId, ForceUpdateTransportRequest request)
    {
        var user = await _accountService.GetUserByIdAsync(request.OwnerId);
        var transport = await _context.Transports.FindAsync(transportId);

        if (user is null || transport is null || transport.OwnerId != user.Id)
            return Result.Failed();

        UpdateTransport(transport, request);
        _context.Transports.Update(transport);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> RemoveAsync(long id, ClaimsPrincipal userClaims)
    {
        var user = await _accountService.GetUserByClaimsAsync(userClaims);
        var transport = await _context.Transports.FindAsync(id);

        if (user is null || transport is null || transport.OwnerId != user.Id)
            return Result.Failed();

        _context.Transports.Remove(transport);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> ForceRemoveAsync(long id)
    {
        var transport = await _context.Transports.FindAsync(id);

        if (transport is null)
            return Result.Failed();

        _context.Transports.Remove(transport);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    private static void UpdateTransport(Transport transport, UpdateTransportRequest request)
    {
        transport.CanBeRented = request.CanBeRented;

        transport.Model = request.Model;
        transport.Color = request.Color;

        transport.Identifier = request.Identifier;
        transport.Description = request.Description;

        transport.Latitude = request.Latitude;
        transport.Longitude = request.Longitude;

        transport.MinutePrice = request.MinutePrice;
        transport.DayPrice = request.DayPrice;

        if (request is ForceUpdateTransportRequest)
        {
            transport.OwnerId = (request as ForceUpdateTransportRequest)!.OwnerId;
            transport.TransportType = (request as ForceUpdateTransportRequest)!.TransportType;
        }
    }

    private static Transport CreateTransport(long ownerId, AddTransportRequest request) => new()
    {
        OwnerId = ownerId,

        CanBeRented = request.CanBeRented,
        TransportType = request.TransportType,

        Model = request.Model,
        Color = request.Color,

        Identifier = request.Identifier,
        Description = request.Description,

        Latitude = request.Latitude,
        Longitude = request.Longitude,

        MinutePrice = request.MinutePrice,
        DayPrice = request.DayPrice,
    };

    private bool IsOwnerOrAdministrator(Transport transport, ApplicationUser user)
        => transport.OwnerId == user.Id && user.IsAdministrator;
}
