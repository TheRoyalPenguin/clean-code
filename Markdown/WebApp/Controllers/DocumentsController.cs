using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.DB.DTO;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly DocumentsService _documentsService;

    public DocumentsController(DocumentsService documentsService)
    {
        _documentsService = documentsService;
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveDocumentAsync([FromForm] DocumentRequest documentRequest)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { Message = "Неверный или отсутствующий идентификатор пользователя." });
        }
        var document = await _documentsService.GetDocumentByIdAsync(documentRequest.Id);
        var isAlreadyExists = document.IsSuccess;
        if (isAlreadyExists)
        {
            await _documentsService.UpdateDocumentAsync(documentRequest, userId);
        }
        else
        {
            await _documentsService.CreateDocumentAsync(documentRequest, userId);
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
            return NotFound(new { Message = documentContent .Error});
        }

        return File(documentContent.Value, "application/octet-stream");
    }
    [HttpGet("myDocuments")]
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

        return BadRequest(documents.Error);
    }
}
