using Simbir.Core.Entities;

namespace Simbir.Core.Requests;

public record GetRentableRequest(double Latitude, double Longitude, double Radius, string TransportType);

public record EndRentRequest (double Latitude, double Longitude);

public record ForceRentRequest (long TransportId, long UserId, DateTime TimeStart, DateTime? TimeEnd, double PriceOfUnit, RentType RentType, double? FinalPrice);