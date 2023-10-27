using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simbir.Core.Entities;

public class Administrator
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId {  get; set; }

    public ApplicationUser? User { get; set; }
}
