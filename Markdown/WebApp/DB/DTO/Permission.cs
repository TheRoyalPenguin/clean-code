using System.ComponentModel.DataAnnotations;
using WebApp.DB.Enums;

namespace WebApp.DB.DTO;

public class Permission
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public AccessLevel AccessLevel { get; set; }
}
