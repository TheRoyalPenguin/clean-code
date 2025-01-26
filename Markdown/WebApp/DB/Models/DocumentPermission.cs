namespace WebApp.DB.Models;

public class DocumentPermission
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid DocumentId { get; set; }
    public Document Document { get; set; }

    public AccessLevel AccessLevel { get; set; }
}
