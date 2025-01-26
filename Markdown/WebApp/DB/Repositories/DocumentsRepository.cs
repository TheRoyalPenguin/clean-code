using Microsoft.EntityFrameworkCore;
using WebApp.DB.Enums;
using WebApp.DB.Models;

namespace WebApp.DB.Repositories;

public class DocumentsRepository
{
    private readonly MyDbContext _dbContext;

    public DocumentsRepository(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> AddDocumentAsync(Document document)
    {
        _dbContext.Documents.Add(document);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
    public async Task<Result> UpdateDocumentAsync(Document document)
    {
        _dbContext.Documents.Update(document);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
    public async Task<Document> GetDocumentByIdAsync(Guid? id)
    {
        var document = await _dbContext.Documents
            .Include(d => d.Permissions)
            .FirstOrDefaultAsync(d => d.id == id);

        return document;
    }
    public async Task<List<Document>> GetDocumentsByUserAsync(Guid userId)
    {
        var documents = await _dbContext.Documents
            .Where(d => d.OwnerId == userId || d.Permissions.Any(p => p.UserId == userId))
            .ToListAsync();

        return documents;
    }
    public async Task<Result> DeleteDocumentAsync(Guid id)
    {
        var document = await GetDocumentByIdAsync(id);
        if (document != null)
        {
            _dbContext.Documents.Remove(document);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }

        return Result.Failure("Ошибка при удалении документа: документ не найден");
    }
    public async Task<Result> AddPermissionAsync(DocumentPermission permission)
    {
        _dbContext.DocumentPermissions.Add(permission);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
    public async Task<Result> RemovePermissionAsync(Guid documentId, Guid userId)
    {
        var documentPermission = await _dbContext.DocumentPermissions
            .FirstOrDefaultAsync(p => p.DocumentId == documentId && p.UserId == userId);
    
        if (documentPermission != null)
        {
            _dbContext.DocumentPermissions.Remove(documentPermission);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }

        return Result.Failure("Ошибка при удалении прав доступа: права доступа не найдены");
    }
    public async Task<Result> CheckReadAccessAsync(Guid documentId, Guid userId)
    {
        var document = await GetDocumentByIdAsync(documentId);

        if (document != null && (document.OwnerId == userId || document.Permissions.Any(p => p.UserId == userId))) 
        {
            return Result.Success();
        }
        return Result.Failure("Ошибка при проверке прав доступа");
    }
    public async Task<Result> CheckWriteAccessAsync(Guid? documentId, Guid userId)
    {
        var document = await GetDocumentByIdAsync(documentId);

        if (document != null && (document.OwnerId == userId || document.Permissions.Any(p => p.UserId == userId && p.AccessLevel == AccessLevel.Write)))
        {
            return Result.Success();
        }
        return Result.Failure("Ошибка при проверке прав доступа");
    }
}
