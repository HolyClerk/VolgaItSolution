using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simbir.Core.Entities;

public class Transport
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Owner))]
    public long OwnerId { get; set; }

    public bool CanBeRented { get; set; }

    public string TransportType { get; set; }
    public string Model { get; set; }
    public string Color { get; set; }
    public string Identifier { get; set; }
    public string? Description { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public double? MinutePrice { get; set; }
    public double? DayPrice { get; set; }

    public virtual ApplicationUser? Owner { get; set; }
}
