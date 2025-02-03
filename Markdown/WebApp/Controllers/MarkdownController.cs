using Microsoft.AspNetCore.Mvc;
using WebApp.DB.DTO;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class MarkdownController : ControllerBase
{
    private readonly IMarkdownService _markdownService;

    public MarkdownController(IMarkdownService markdownService)
    {
        _markdownService = markdownService;
    }
    
    [HttpPost("convert")]
    public async Task<IActionResult> ConvertMarkdownToHtml([FromBody] MarkdownRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.InputText))
        {
            return BadRequest("Текст маркдаун разметки не может быть пустым.");
        }

        var htmlText = await _markdownService.GetHtml(request.InputText);
        return Ok(new { HtmlText = htmlText.Value });
    }
}
