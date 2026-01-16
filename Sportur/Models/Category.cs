using System.ComponentModel.DataAnnotations;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    public int? ParentCategoryId { get; set; }
    public Category ParentCategory { get; set; }

    public ICollection<Category> Children { get; set; } = new List<Category>();
}
