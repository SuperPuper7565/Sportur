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

    [Required]
    public byte[] PasswordSalt { get; set; }
    [Required]
    public UserRole Role { get; set; }

    public bool IsWholesaleApproved { get; set; }
}
