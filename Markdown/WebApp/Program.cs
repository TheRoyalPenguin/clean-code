using System.Text.Json;
using Markdown.BaseClasses;

namespace WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapPost("/markdown-to-html-convert", async (HttpContext context) =>
        {
            var requestBody = await JsonSerializer.DeserializeAsync<MarkdownRequest>(context.Request.Body);

            if (requestBody == null || string.IsNullOrWhiteSpace(requestBody.InputText))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Текст не может быть пустым.");
                return;
            }

            MarkdownToHtmlRenderer renderer = new MarkdownToHtmlRenderer();
            var htmlText = renderer.Render(requestBody.InputText);

            var response = new
            {
                HtmlText = htmlText
            };

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        });

        app.Run();
    }
}
