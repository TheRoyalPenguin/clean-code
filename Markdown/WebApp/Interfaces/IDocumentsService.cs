using WebApp.DB.DTO;
using WebApp.DB.Models;

namespace WebApp.Interfaces;

public interface IDocumentsService
{
    Task<Result<DocumentDto>> CreateDocumentAsync(DocumentRequest documentRequest, Guid userId);
    Task<Result<DocumentDto>> UpdateDocumentAsync(DocumentRequest documentRequest, Guid userId);
    Task<Result> DeleteDocumentAsync(Guid documentid, Guid userId);
    Task<Result<Document>> GetDocumentByIdAsync(Guid? id);
    Task<Result<List<User>>> GetUsersWithReadPermissionAsync(Guid documentId);
    Task<Result<List<User>>> GetUsersWithWritePermissionAsync(Guid documentId);
    Task<Result<Stream>> GetDocumentContentAsync(Guid documentId, Guid userId);
    Task<Result<List<Document>>> GetDocumentsByUserAsync(Guid userId);
    Task<Result<List<Document>>> GetAvailableDocumentsToUserAsync(Guid userId);
    Task<Result> AddPermissionAsync(Guid documentId, Guid userId, Permission permission);
    Task<Result> RemovePermissionAsync(Guid documentid, Guid ownerUserId, Guid userId);
}
