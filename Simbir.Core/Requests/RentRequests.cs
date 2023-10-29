namespace Simbir.Core.Requests;

public record GetRentableRequest(double Latitude, double Longitude, double Radius, string Type);

public record EndRentRequest (double Latitude, double Longitude);
public record StartRentRequest (string RentType);

public record ForceUpdateRentRequest (long TransportId, long UserId, string RentType);