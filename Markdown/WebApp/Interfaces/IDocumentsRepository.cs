using WebApp.DB.Models;

namespace WebApp.Interfaces;

public interface IDocumentsRepository
{
    Task<Result> AddDocumentAsync(Document document);
    Task<Result> UpdateDocumentAsync(Document document);
    Task<Document> GetDocumentByIdAsync(Guid? id);
    Task<List<Document>> GetDocumentsByUserAsync(Guid userId);
    Task<List<Document>> GetAvailableDocumentsToUserAsync(Guid userId);
    Task<List<User>> GetUsersWithReadPermissionAsync(Guid documentId);
    Task<List<User>> GetUsersWithWritePermissionAsync(Guid documentId);
    Task<Result> DeleteDocumentAsync(Guid id);
    Task<Result> AddPermissionAsync(DocumentPermission permission);
    Task<Result> RemovePermissionAsync(Guid documentId, Guid userId);
    Task<Result> CheckReadAccessAsync(Guid documentId, Guid userId);
    Task<Result> CheckWriteAccessAsync(Guid? documentId, Guid userId);
}
