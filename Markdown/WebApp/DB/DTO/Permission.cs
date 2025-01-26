using System.ComponentModel.DataAnnotations;

namespace WebApp.DB.DTO;

public class Permission
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public AccessLevel AccessLevel { get; set; }
}
