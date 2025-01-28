using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.DB.DTO;
using WebApp.DB.Models;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly DocumentsService _documentsService;
    private readonly UserService _userService;

    public DocumentsController(DocumentsService documentsService, UserService userService)
    {
        _documentsService = documentsService;
        _userService = userService;
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveDocumentAsync([FromForm] DocumentRequest documentRequest)
    {
        if (documentRequest == null)
        {
            return BadRequest("Пустое тело запроса.");
        }
        if (!ModelState.IsValid) // валидация с помощью атрибутов
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Неверный или отсутствующий идентификатор пользователя.");
        }
        Result<DocumentDto> result;
        if (documentRequest.Id != null)
        {
            var document = await _documentsService.GetDocumentByIdAsync(documentRequest.Id);
            var isAlreadyExists = document.IsSuccess;
            if (isAlreadyExists)
            {
                result = await _documentsService.UpdateDocumentAsync(documentRequest, userId);
            }
            else
            {
                result = await _documentsService.CreateDocumentAsync(documentRequest, userId);
            }
        }
        else
        {
            result = await _documentsService.CreateDocumentAsync(documentRequest, userId);
        }
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        else
        {
            return Unauthorized(result);
        }
    }
    [HttpPost("{documentId}/delete")]
    public async Task<IActionResult> DeleteDocumentAsync(Guid documentId)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { Message = "Неверный или отсутствующий идентификатор пользователя." });
        }
        var documentContent = await _documentsService.DeleteDocumentAsync(documentId, userId);

        if (!documentContent.IsSuccess)
        {
            return NotFound(new { Message = documentContent.ErrorMessage });
        }

        return Ok();
    }
    [HttpGet("{documentId}/content")]
    public async Task<IActionResult> GetDocumentContentAsync(Guid documentId)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { Message = "Неверный или отсутствующий идентификатор пользователя." });
        }

        var documentContent = await _documentsService.GetDocumentContentAsync(documentId, userId);

        if (documentContent.Value == null)
        {
            return NotFound(new { Message = documentContent.ErrorMessage});
        }

        return File(documentContent.Value, "application/octet-stream");
    }
    [HttpGet("my")]
    public async Task<IActionResult> GetDocumentsByUser()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { Message = "Неверный или отсутствующий идентификатор пользователя." });
        }
        var documents = await _documentsService.GetDocumentsByUserAsync(userId);

        if (documents.IsSuccess)
        {
            return Ok(documents.Value);
        }

        return BadRequest(documents.ErrorMessage);
    }
    [HttpGet("available")]
    public async Task<IActionResult> GetDocumentsAvailableToUser()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { Message = "Неверный или отсутствующий идентификатор пользователя." });
        }
        var documents = await _documentsService.GetAvailableDocumentsToUserAsync(userId);

        if (documents.IsSuccess)
        {
            return Ok(documents.Value);
        }

        return BadRequest(documents.ErrorMessage);
    }
    [HttpPost("{documentId}/permissions/set")]
    public async Task<IActionResult> AddPermission(Guid documentId, [FromForm] Permission permissionRequest)
    {
        if (permissionRequest == null)
        {
            return BadRequest(new { Message = "Пустое тело запроса." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new { Message = ModelState });
        }
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { Message = "Неверный или отсутствующий идентификатор пользователя." });
        }
        var result = await _documentsService.AddPermissionAsync(documentId, userId, permissionRequest);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
    [HttpPost("{documentId}/permissions/remove")]
    public async Task<IActionResult> RemovePermission(Guid documentId, [FromBody] RemoveAccessRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { Message = "Пустое тело запроса." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new { Message = ModelState });
        }
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { Message = "Неверный или отсутствующий идентификатор пользователя." });
        }
        var result = await _documentsService.RemovePermissionAsync(documentId, userId, request.UserId);

        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(result.ErrorMessage);
    }
    [HttpGet("{documentId}/users-with-read-permission")]
    public async Task<IActionResult> GetUsersWithReadPermission(Guid documentId)
    {
        var result = await _documentsService.GetUsersWithReadPermissionAsync(documentId);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(new { Message = result.ErrorMessage });
    }

    [HttpGet("{documentId}/users-with-write-permission")]
    public async Task<IActionResult> GetUsersWithWritePermission(Guid documentId)
    {
        var result = await _documentsService.GetUsersWithWritePermissionAsync(documentId);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(new { Message = result.ErrorMessage });
    }
}
