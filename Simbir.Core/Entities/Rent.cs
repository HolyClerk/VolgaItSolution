using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simbir.Core.Entities;

public class Rent 
{
    [Key]
    public long Id { get; set; }

    [ForeignKey(nameof(Renter))]
    public long RenterId { get; set; }

    [ForeignKey(nameof(RentedTransport))]
    public long TransportId { get; set; }

    public RentType Type { get; set; }
    public bool IsRentEnded { get; set; }

    public DateTime RentStarted { get; set; }
    public DateTime? RentEnded { get; set; }

    public double? FinalPrice { get; set; }
    public double PriceOfUnit { get; set; }

    public virtual ApplicationUser Renter { get; set; }
    public virtual Transport RentedTransport { get; set; }
}

public enum RentType
{
    Days,
    Minutes
}
