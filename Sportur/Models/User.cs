using System.ComponentModel.DataAnnotations;

public enum UserRole
{
    Retail,
    Wholesale,
    Admin
}

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Surname { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    public UserRole Role { get; set; }
}
