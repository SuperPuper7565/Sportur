using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Sportur.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sportur.Models
{
    public class WholesalePrice
    {
        public int Id { get; set; }

        [Required]
        public int BicycleVariantId { get; set; }

        [ValidateNever]
        public BicycleVariant BicycleVariant { get; set; }

        [Required]
        public int UserId { get; set; }

        [ValidateNever]
        public User User { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}

