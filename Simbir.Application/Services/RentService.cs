using System.Diagnostics.Metrics;
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
                {
                    return false;
                }

                if (request.Type.ToLower() != transport.TransportType && request.Type.ToLower() != "all")
                {
                    return false;
                }

                // Координата транспорта
                var transportCoordinate = new GeoCoordinate(transport.Latitude, transport.Longitude); 
                var distance = centerCoordinate.GetDistanceTo(transportCoordinate);
                return distance <= request.Radius;
            })
            .ToList();

        return Result<List<Transport>>.Success(transportsInRadius);
    }

    public async Task<Result<Rent>> GetAsync(long rentId, ClaimsPrincipal claims)
    {
        var requester = await _accountService.GetUserByClaimsAsync(claims);
        var rent = await _context.Rents
            .Include(rent => rent.RentedTransport)
            .FirstOrDefaultAsync(rent => rent.Id == rentId);

        if (requester is null || rent is null)
        {
            return Result<Rent>.Failed("Запрошеная аренда не найдена или вы не являетесь арендатором/владельцем транспорта");
        }

        if (rent.RenterId == requester.Id|| rent.RentedTransport.OwnerId == requester.Id)
        {
            return Result<Rent>.Success(rent);
        }

        return Result<Rent>.Failed("Запрошеная аренда не найдена или вы не являетесь арендатором/владельцем транспорта");
    }

    public async Task<Result<Rent>> ForceGetAsync(long rentId)
    {
        var rent = await _context.Rents.FindAsync(rentId);

        return rent switch
        {
            Rent => Result<Rent>.Success(rent),
            null => Result<Rent>.Failed("Не найдена аренда"),
        };
    }

    public async Task<Result<List<Rent>>> GetAccountHistoryAsync(ClaimsPrincipal claims)
    {
        var requester = await _accountService.GetUserByClaimsAsync(claims);

        if (requester is null)
        {
            return Result<List<Rent>>.Failed();
        }

        var rents = _context.Rents.Where(rent => rent.RenterId == requester.Id);
        return Result<List<Rent>>.Success(await rents.ToListAsync());
    }

    public async Task<List<Rent>> ForceGetAccountHistoryAsync(long userId)
    {
        return await _context.Rents
            .Where(r => r.RenterId == userId)
            .ToListAsync();
    }

    public async Task<Result<List<Rent>>> GetTransportHistoryAsync(long transportId, ClaimsPrincipal claims)
    {
        var requester = await _accountService.GetUserByClaimsAsync(claims);
        var transport = await _context.Transports.FindAsync(transportId);

        if (requester is null || transport is null || transport.OwnerId != requester.Id)
            return Result<List<Rent>>.Failed();

        var rents = _context.Rents.Where(rent => rent.TransportId == transport.Id);
        return Result<List<Rent>>.Success(await rents.ToListAsync());
    }

    public async Task<List<Rent>> ForceGetTransportHistoryAsync(long transportId)
    {
        return await _context.Rents
            .Where(r => r.TransportId == transportId)
            .ToListAsync();
    }
    public async Task<Result> ForceUpdateAsync(long rentId, ForceUpdateRentRequest request)
    {
        var rent = await _context.Rents
            .Include(rent => rent.RentedTransport)
            .FirstOrDefaultAsync(rent => rent.Id == rentId);

        if (rent is null)
        {
            return Result.Failed();
        }

        rent.TransportId = request.TransportId;
        rent.RenterId = request.UserId;
        rent.Type = request.RentType;

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> RentAsync(long transportId, string rentType, ClaimsPrincipal renterClaims)
    {
        var renter = await _accountService.GetUserByClaimsAsync(renterClaims);
        var transport = await _context.Transports.FindAsync(transportId);

        if (renter is null || transport is null || renter.Id == transport.OwnerId)
            return Result.Failed();

        transport.CanBeRented = false;

        await _context.Rents.AddAsync(new Rent()
        {
            TransportId = transportId,
            RenterId = renter.Id,
            @Type = rentType,
        });

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> EndRentAsync(long rentId, EndRentRequest request, ClaimsPrincipal renterClaims)
    {
        var renter = await _accountService.GetUserByClaimsAsync(renterClaims);

        var rent = await _context.Rents
            .Include(rent => rent.RentedTransport)
            .FirstOrDefaultAsync(rent => rent.Id == rentId);

        if (renter is null || rent is null || rent.RenterId != renter.Id)
        {
            return Result.Failed();
        }

        rent.IsRentEnded = true;
        rent.RentedTransport.CanBeRented = true;
        rent.RentedTransport.Latitude = request.Latitude;
        rent.RentedTransport.Longitude = request.Longitude;

        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> ForceEndRentAsync(long rentId, EndRentRequest request)
    {
        var rent = await _context.Rents
            .Include(rent => rent.RentedTransport)
            .FirstOrDefaultAsync(rent => rent.Id == rentId);

        if (rent is null)
        {
            return Result.Failed("Аренда не найдена");
        }

        rent.IsRentEnded = true;
        rent.RentedTransport.CanBeRented = true;
        rent.RentedTransport.Latitude = request.Latitude;
        rent.RentedTransport.Longitude = request.Longitude;

        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> ForceRemoveRentAsync(long rentId)
    {
        var rent = await _context.Rents.FindAsync(rentId);

        if (rent is null)
        {
            return Result.Failed("Аренда не найдена");
        }

        _context.Rents.Remove(rent);
        await _context.SaveChangesAsync();

        return Result.Success();
    }
}
