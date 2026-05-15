using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Arket.DTOs;
using Arket.Models;

namespace Arket.Services;

public class CvPdfService
{
    public byte[] GenerateCvPdf(User user, GenerateCvDto? dto = null)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var aboutMe   = dto?.AboutMe   ?? "";
        var skills    = dto?.Skills    ?? "";
        var education = dto?.Education ?? "";
        var languages = dto?.Languages ?? "";

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);

                page.Content().Column(col =>
                {
                    col.Item()
                        .Text($"{user.FirstName} {user.LastName}")
                        .FontSize(28).Bold();
                    
                    col.Item().Text(user.Email);
                    col.Item().Text(user.PhoneNumber);
                    
                    if (!string.IsNullOrWhiteSpace(aboutMe))
                    {
                        col.Item().PaddingTop(20)
                            .Text("PROFILE").FontSize(18).Bold();
                        col.Item().Text(aboutMe);
                    }
                    
                    if (!string.IsNullOrWhiteSpace(skills))
                    {
                        col.Item().PaddingTop(20)
                            .Text("SKILLS").FontSize(18).Bold();
                        foreach (var skill in Split(skills))
                            col.Item().Text($"• {skill}");
                    }
                    
                    if (!string.IsNullOrWhiteSpace(languages))
                    {
                        col.Item().PaddingTop(20)
                            .Text("LANGUAGES").FontSize(18).Bold();
                        foreach (var lang in Split(languages))
                            col.Item().Text($"• {lang}");
                    }
                    
                    if (!string.IsNullOrWhiteSpace(education))
                    {
                        col.Item().PaddingTop(20)
                            .Text("EDUCATION").FontSize(18).Bold();
                        col.Item().Text(education);
                    }
                });
            });
        });

        return document.GeneratePdf();
    }

    private static string[] Split(string value) =>
        value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}