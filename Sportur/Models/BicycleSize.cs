using Sportur.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class BicycleSize
{
    [Key]
    public int Id { get; set; }

    public int BicycleModelId { get; set; }
    public BicycleModel BicycleModel { get; set; }

    [Required]
    [StringLength(20)]
    public string FrameSize { get; set; }


    public int StockQuantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
}
