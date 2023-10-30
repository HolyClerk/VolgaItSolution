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

                if (request.TransportType.ToLower() != transport.TransportType
                    && request.TransportType.ToLower() != "all")
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

        if (rent.RenterId == requester.Id || rent.RentedTransport.OwnerId == requester.Id)
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
            return Result<List<Rent>>.Failed("Непредвиденная ошибка", "Claims не найдены");
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
            return Result<List<Rent>>.Failed("Транспорт не был найден или вы не являетесь его владельцем");

        var rents = _context.Rents.Where(rent => rent.TransportId == transport.Id);
        return Result<List<Rent>>.Success(await rents.ToListAsync());
    }

    public async Task<List<Rent>> ForceGetTransportHistoryAsync(long transportId)
    {
        return await _context.Rents
            .Where(r => r.TransportId == transportId)
            .ToListAsync();
    }
    public async Task<Result> ForceUpdateAsync(long rentId, ForceRentRequest request)
    {
        var rent = await _context.Rents
            .Include(rent => rent.RentedTransport)
            .FirstOrDefaultAsync(rent => rent.Id == rentId);

        var renter = await _accountService.GetUserByIdAsync(request.UserId);
        var transport = await _context.Transports.FindAsync(request.TransportId);

        if (rent is null || renter is null || transport is null)
        {
            return Result.Failed("Аренда, арендодатель или транспорт не были найдены");
        }

        ForceUpdateRent(rent, request);

        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> RentAsync(long transportId, RentType rentType, ClaimsPrincipal renterClaims)
    {
        var renter = await _accountService.GetUserByClaimsAsync(renterClaims);
        var transport = await _context.Transports.FindAsync(transportId);

        if (renter is null || transport is null || renter.Id == transport.OwnerId)
        {
            return Result.Failed("Транспорт не был найден, или вы являетесь его владельцем");
        }

        if (transport.CanBeRented is false)
        {
            return Result.Failed("Транспорт уже арендован");
        }

        transport.CanBeRented = false;

        await _context.Rents.AddAsync(new Rent()
        {
            RentStarted = DateTime.UtcNow,
            PriceOfUnit = rentType is RentType.Minutes ? (double)transport.MinutePrice! : (double)transport.DayPrice!,
            TransportId = transportId,
            RenterId = renter.Id,
            @Type = rentType,
        });

        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> ForceRentAsync(ForceRentRequest request)
    {
        var renter = await _accountService.GetUserByIdAsync(request.UserId);
        var transport = await _context.Transports.FindAsync(request.TransportId);

        if (renter is null || transport is null)
        {
            return Result.Failed("Аренда или арендатель не были найдены");
        }

        await _context.Rents.AddAsync(new Rent()
        {
            RentStarted = DateTime.UtcNow,
            RentEnded = request.TimeEnd,
            PriceOfUnit = request.PriceOfUnit,
            TransportId = transport.Id,
            RenterId = renter.Id,
            @Type = request.RentType,
            IsRentEnded = request.TimeEnd.HasValue,
            FinalPrice = request.FinalPrice,
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
            return Result.Failed("Аренда не была найдена или вы не являетесь арендателем");
        }

        CloseRent(rent, request);

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

        CloseRent(rent, request);

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

    private static void CloseRent(Rent rent, EndRentRequest request)
    {
        rent.IsRentEnded = true;
        rent.RentEnded = DateTime.UtcNow;
        rent.RentedTransport.CanBeRented = true;
        rent.RentedTransport.Latitude = request.Latitude;
        rent.RentedTransport.Longitude = request.Longitude;

        rent.FinalPrice = rent.Type switch
        {
            RentType.Minutes => CalculateRentCostByMinutes(rent.PriceOfUnit, rent.RentStarted, DateTime.UtcNow),
            RentType.Days => CalculateRentCostByDays(rent.PriceOfUnit, rent.RentStarted, DateTime.UtcNow),
            _ => CalculateRentCostByMinutes(rent.PriceOfUnit, rent.RentStarted, DateTime.UtcNow),
        };
    }

    private static double CalculateRentCostByMinutes(double pricePerMinute, DateTime rentStart, DateTime rentEnd)
    {
        TimeSpan rentalDuration = rentEnd - rentStart; 
        var totalCost = pricePerMinute * rentalDuration.TotalMinutes; 
        return totalCost;
    }

    private static double CalculateRentCostByDays(double pricePerDay, DateTime rentStart, DateTime rentEnd)
    {
        TimeSpan rentalDuration = rentStart - rentEnd; 
        var totalDays = Math.Ceiling(rentalDuration.TotalDays);
        var totalCost = pricePerDay * totalDays;

        if (totalDays < 1)
            return pricePerDay;

        return totalCost;
    }

    private static void ForceUpdateRent(Rent rent, ForceRentRequest request)
    {
        rent.TransportId = request.TransportId;
        rent.RenterId = request.UserId;
        rent.RentStarted = request.TimeStart;
        rent.RentEnded = request.TimeEnd;
        rent.PriceOfUnit = request.PriceOfUnit;
        rent.Type = request.RentType;
        rent.FinalPrice = request.FinalPrice;
    }
}
