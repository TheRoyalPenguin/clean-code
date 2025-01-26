using WebApp.DB.DTO;
using WebApp.DB.Enums;
using WebApp.DB.Models;
using WebApp.DB.Repositories;
using WebApp.Interfaces;

namespace WebApp.Services;

public class DocumentsService
{
    private readonly DocumentsRepository _documentsRepository;
    private readonly UsersRepository _usersRepository;
    private readonly IFileStorageService _fileStorageService;
    public DocumentsService(DocumentsRepository documentsRepository, UsersRepository usersRepository, IFileStorageService fileStorageService)
    {
        _documentsRepository = documentsRepository;
        _usersRepository = usersRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Document>> CreateDocumentAsync(DocumentRequest documentRequest, Guid userId)
    {
        using var fileStream = documentRequest.File.OpenReadStream();
        var objectName = await _fileStorageService.UploadFileAsync(
            documentRequest.File.FileName,
            documentRequest.File.ContentType,
            fileStream
            );

        var document = new Document
        {
            Title = documentRequest.Title,
            Name = documentRequest.File.Name,
            ContentType = documentRequest.File.ContentType,
            FileSize = documentRequest.File.Length,
            StorageObjectId = objectName,
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var isAdded = await _documentsRepository.AddDocumentAsync(document);
        if (isAdded.IsSuccess)
        {
            return Result<Document>.Success(document);
        }

        return Result<Document>.Failure("Не удалось создать документ.");
    }
    public async Task<Result<Document>> UpdateDocumentAsync(DocumentRequest documentRequest, Guid userId)
    {
        var document = await _documentsRepository.GetDocumentByIdAsync(documentRequest.Id);
        if (document == null)
        {
            return Result<Document>.Failure($"Документ {documentRequest.Id} не найден.");
        }

        var writeAccessCheckedResult = await _documentsRepository.CheckWriteAccessAsync(documentRequest.Id, userId);
        if (!writeAccessCheckedResult.IsSuccess)
        {
            return Result<Document>.Failure("Нет доступа к изменениям.");
        }

        await _fileStorageService.DeleteFileAsync(document.StorageObjectId);

        using var fileStream = documentRequest.File.OpenReadStream();
        var newObjectName = await _fileStorageService.UploadFileAsync(
            documentRequest.File.FileName,
            documentRequest.File.ContentType,
            fileStream
            );

        document.Title = documentRequest.Title;
        document.Name = documentRequest.File.FileName;
        document.ContentType = documentRequest.File.ContentType;
        document.FileSize = documentRequest.File.Length;
        document.StorageObjectId = newObjectName;
        document.LastModifiedAt = DateTime.UtcNow;

        await _documentsRepository.UpdateDocumentAsync(document);
        return Result<Document>.Success(document);
    }
    public async Task<Result> DeleteDocumentAsync(Guid userId, Guid documentid)
    {
        var document = await _documentsRepository.GetDocumentByIdAsync(documentid);
        if (document == null)
        {
            return Result.Failure("Документ не найден.");
        }
        if (document.OwnerId != userId)
        {
            return Result.Failure("Удалить документ может только его владелец.");
        }
        await _fileStorageService.DeleteFileAsync(document.StorageObjectId);
        await _documentsRepository.DeleteDocumentAsync(documentid);

        return Result.Success();
    }
    public async Task<Result<Document>> GetDocumentByIdAsync(Guid? id)
    {
        var document = await _documentsRepository.GetDocumentByIdAsync(id);

        if (document == null)
        {
            return Result<Document>.Failure("Документ не найден.", Errors.NotFound);
        }

        return Result<Document>.Success(document);
    }
    public async Task<Result<Stream>> GetDocumentContentAsync(Guid documentId, Guid userId)
    {
        var document = await _documentsRepository.GetDocumentByIdAsync(documentId);
        if (document == null)
        {
            return Result<Stream>.Failure("Документ не найден.");
        }

        var hasReadAccess = await _documentsRepository.CheckReadAccessAsync(documentId, userId);
        if (!hasReadAccess.IsSuccess)
        {
            return Result<Stream>.Failure("Нет прав доступа на чтение документа.");
        }

        var fileStream = await _fileStorageService.GetFileAsync(document.StorageObjectId);
        if (fileStream == null)
        {
            return Result<Stream>.Failure("Файл не найден в хранилище.");
        }

        return Result<Stream>.Success(fileStream);
    }
    public async Task<Result<List<Document>>> GetDocumentsByUserAsync(Guid userId)
    {
        var documents = await _documentsRepository.GetDocumentsByUserAsync(userId);
        if (documents != null && documents.Any())
        {
            return Result<List<Document>>.Success(documents);
        }

        return Result<List<Document>>.Failure("Документы не найдены.");
    }
    public async Task<Result> AddPermissionAsync(Guid documentId, Guid userId, Permission permission)
    {
        var document = await _documentsRepository.GetDocumentByIdAsync(documentId);
        if (document == null)
        {
            return Result.Failure("Документ не найден.");
        }
        if (document.OwnerId != userId)
        {
            return Result.Failure("Изменять права доступа у документа может только его владелец.");
        }

        var user = await _usersRepository.GetByEmailAsync(permission.Email);
        if (user == null)
        {
            return Result.Failure("Пользователь с указанным email не найден.");
        }

        var existingPermission = await _documentsRepository.CheckReadAccessAsync(documentId, user.Id);
        if (existingPermission != null)
        {
            return Result.Failure("Права доступа для этого пользователя уже выданы.");
        }

        var documentPermission = new DocumentPermission
        {
            DocumentId = documentId,
            UserId = user.Id,
            AccessLevel = permission.AccessLevel
        };

        await _documentsRepository.AddPermissionAsync(documentPermission);
        return Result.Success();
    }
    public async Task<Result> RemovePermissionAsync(Guid documentid, Guid ownerUserId, Guid userId)
    {
        var document = await _documentsRepository.GetDocumentByIdAsync(documentid);
        if (document == null)
        {
            return Result.Failure("Документ не найден.");
        }
        if (document.OwnerId != ownerUserId)
        {
            return Result.Failure("Изменять права доступа у документа может только его владелец.");
        }

        await _documentsRepository.RemovePermissionAsync(documentid, userId);
        return Result.Success();
    }
}
