using Sportur.Models;
using System.ComponentModel.DataAnnotations;

public class BicycleColor
{
    [Key]
    public int Id { get; set; }

    public int BicycleModelId { get; set; }
    public BicycleModel BicycleModel { get; set; }

    [Required]
    [StringLength(50)]
    public string Color { get; set; }

    [StringLength(500)]
    public string PhotoUrl { get; set; }

}
