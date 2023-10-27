using System.Security.Claims;
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

    public async Task<Result<Transport>> GetTransportAsync(int id)
    {
        var transport = await _context.Transports.FindAsync(id);

        if (transport is null)
            return Result<Transport>.Failed("Транспорт с id \"{id}\" не найден.");

        return Result<Transport>.Success(transport);
    }

    public async Task<Result> AddTransportAsync(AddTransportRequest request, ClaimsPrincipal claims)
    {
        var applicationUser = _accountService.GetUserByClaimsAsync(claims);

        if (applicationUser is null)
            return Result.Failed();

        var newTransport = CreateTransport(applicationUser.Id, request);
        await _context.Transports.AddAsync(newTransport);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateTransportAsync(int transportId, UpdateTransportRequest request, ClaimsPrincipal claims)
    {
        var applicationUser = _accountService.GetUserByClaimsAsync(claims);
        var transport = await _context.Transports.FindAsync(transportId);

        if (applicationUser is null || transport is null || transport.OwnerId != applicationUser.Id)
            return Result.Failed();

        UpdateTransport(transport, request);
        _context.Transports.Update(transport);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> RemoveTransportAsync(int id, ClaimsPrincipal claims)
    {
        var applicationUser = _accountService.GetUserByClaimsAsync(claims);
        var transport = await _context.Transports.FindAsync(id);

        if (applicationUser is null || transport is null || transport.OwnerId != applicationUser.Id)
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
    }

    private static Transport CreateTransport(int ownerId, AddTransportRequest request) => new Transport
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
}
