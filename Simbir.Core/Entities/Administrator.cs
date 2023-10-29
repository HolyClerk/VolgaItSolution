using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simbir.Core.Entities;

public class Administrator
{
    [Key]
    public long Id { get; set; }

    [ForeignKey(nameof(User))]
    public long UserId {  get; set; }

    public ApplicationUser? User { get; set; }
}
