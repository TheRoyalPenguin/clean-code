using System.ComponentModel.DataAnnotations;

namespace WebApp.DB.DTO;

public class DocumentRequest
{
    [Required]
    public Guid? Id { get; set; }
    [Required]
    public string Title { get; set; }

    [Required]
    public IFormFile File { get; set; }
}
