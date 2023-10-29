using System.Security.Claims;
using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using Simbir.Application.Abstractions;
using Simbir.Application.Other;
using Simbir.Core.Entities;
using Simbir.Core.Requests;
using Simbir.Core.Results;

namespace Simbir.Application.Services;

public class RentService : IRentService
{
    private readonly IDbContext _context;
    private readonly IAccountService _accountService;

    public RentService(IDbContext context, 
        IAccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    // TODO: Переделать в асинхронный вариант
    public Result<List<Transport>> GetRentableTransport(GetRentableRequest request)
    {
        // Координата поиска
        var centerCoordinate = new GeoCoordinate(request.Latitude, request.Longitude);

        var transportsInRadius = _context.Transports
            .AsEnumerable()
            .Where(transport =>
            {
                if (transport.CanBeRented is false)
                    return false;

                // Координата транспорта
                var transportCoordinate = new GeoCoordinate(transport.Latitude, transport.Longitude); 
                var distance = centerCoordinate.GetDistanceTo(transportCoordinate);
                return distance <= request.Radius * 1000; // Перевод в метры, если указано в км
            })
            .ToList();

        return Result<List<Transport>>.Success(transportsInRadius);
    }

    public async Task<Result<Rent>> GetRentAsync(int rentId, ClaimsPrincipal claims)
    {
        var requester = await _accountService.GetUserByClaimsAsync(claims);
        var rent = await _context.Rents
            .Include(rent => rent.RentedTransport)
            .FirstOrDefaultAsync(rent => rent.Id == rentId);

        if (requester is null || rent is null)
            return Result<Rent>.Failed("Запрошеная аренда не найдена или вы не являетесь арендатором/владельцем транспорта");

        if (rent.RenterId == requester.Id|| rent.RentedTransport.OwnerId == requester.Id)
            return Result<Rent>.Success(rent);

        return Result<Rent>.Failed("Запрошеная аренда не найдена или вы не являетесь арендатором/владельцем транспорта");
    }

    public async Task<Result<List<Rent>>> GetAccountHistoryAsync(ClaimsPrincipal claims)
    {
        var requester = await _accountService.GetUserByClaimsAsync(claims);

        if (requester is null)
            return Result<List<Rent>>.Failed();

        var rents = _context.Rents.Where(rent => rent.RenterId == requester.Id);
        return Result<List<Rent>>.Success(await rents.ToListAsync());
    }

    public async Task<Result<List<Rent>>> GetTransportHistoryAsync(int transportId, ClaimsPrincipal claims)
    {
        var requester = await _accountService.GetUserByClaimsAsync(claims);
        var transport = await _context.Transports.FindAsync(transportId);

        if (requester is null || transport is null || transport.OwnerId == requester.Id)
            return Result<List<Rent>>.Failed();

        var rents = _context.Rents.Where(rent => rent.TransportId == transport.Id);
        return Result<List<Rent>>.Success(await rents.ToListAsync());
    }

    public async Task<Result> StartNewRentAsync(int transportId, string rentType, ClaimsPrincipal renterClaims)
    {
        var renter = await _accountService.GetUserByClaimsAsync(renterClaims);
        var transport = await _context.Transports.FindAsync(transportId);

        if (renter is null || transport is null)
            return Result.Failed();

        transport.CanBeRented = false;

        await _context.Rents.AddAsync(new Rent()
        {
            TransportId = transportId,
            RenterId = (int)renter.Id,
            @Type = rentType,
        });

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> EndRentAsync(int rentId, EndRentRequest request, ClaimsPrincipal renterClaims)
    {
        var renter = await _accountService.GetUserByClaimsAsync(renterClaims);

        var rent = await _context.Rents
            .Include(rent => rent.RentedTransport)
            .FirstOrDefaultAsync(rent => rent.Id == rentId);

        if (renter is null || rent is null || rent.RenterId != renter.Id)
            return Result.Failed();

        rent.IsRentEnded = true;
        rent.RentedTransport.CanBeRented = true;
        rent.RentedTransport.Latitude = request.Latitude;
        rent.RentedTransport.Longitude = request.Longitude;

        await _context.SaveChangesAsync();
        return Result.Success();
    }
}
