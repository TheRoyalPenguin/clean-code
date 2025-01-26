namespace WebApp.DB.Models;

public class Document
{
    public Guid id { get; set; }
    public string Title { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
    public string StorageObjectId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }


    public Guid OwnerId { get; set; }
    public User Owner { get; set; }

    public List<DocumentPermission> Permissions { get; set; }
}
