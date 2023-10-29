using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simbir.Core.Entities;

public class Rent 
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Renter))]
    public int RenterId { get; set; }

    [ForeignKey(nameof(RentedTransport))]
    public int TransportId { get; set; }

    public string Type { get; set; }
    public bool IsRentEnded { get; set; }

    public virtual ApplicationUser Renter { get; set; }
    public virtual Transport RentedTransport { get; set; }
}
