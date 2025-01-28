namespace WebApp.DB.DTO;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
    public string StorageObjectId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
}