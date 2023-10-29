namespace Simbir.Core.Requests;

public class AddTransportRequest
{
    public bool CanBeRented { get; set; }
    public string TransportType { get; set; }
    public string Model { get; set; }
    public string Color { get; set; }
    public string Identifier { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? MinutePrice { get; set; }
    public double? DayPrice { get; set;}
}

public class ForceAddTransportRequest : AddTransportRequest
{
    public long OwnerId { get; set; }
}

public class UpdateTransportRequest
{
    public bool CanBeRented { get; set; }
    public string Model { get; set; }
    public string Color { get; set; }
    public string Identifier { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? MinutePrice { get; set; }
    public double? DayPrice { get; set; }
}

public class ForceUpdateTransportRequest : UpdateTransportRequest
{
    public long OwnerId { get; set; }
    public string TransportType { get; set; }
}
