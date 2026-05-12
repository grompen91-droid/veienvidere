using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Arket.Models;

namespace Arket.Services;

public class CvPdfService
{
    public byte[] GenerateCvPdf(User user, string template)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        switch (template.ToLower())
        {
            case "modern":
                return GenerateModernTemplate(user);

            case "minimal":
                return GenerateMinimalTemplate(user);

            case "professional":
                return GenerateProfessionalTemplate(user);

            default:
                return GenerateModernTemplate(user);
        }
    }

    private byte[] GenerateModernTemplate(User user)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);

                page.Content()
                    .Column(column =>
                    {
                        column.Item()
                            .Text($"{user.FirstName} {user.LastName}")
                            .FontSize(28)
                            .Bold();

                        column.Item().Text(user.Email);
                        column.Item().Text(user.PhoneNumber);

                        column.Item().PaddingTop(20);

                        column.Item()
                            .Text("PROFILE")
                            .FontSize(18)
                            .Bold();

                        column.Item().Text(
                            "Motivated and ambitious backend developer with strong interest in ASP.NET Core, PostgreSQL, Docker and scalable web applications.");

                        column.Item().PaddingTop(20);

                        column.Item()
                            .Text("SKILLS")
                            .FontSize(18)
                            .Bold();

                        column.Item().Text("• C#");
                        column.Item().Text("• ASP.NET Core");
                        column.Item().Text("• PostgreSQL");
                        column.Item().Text("• Docker");
                        column.Item().Text("• Entity Framework Core");

                        column.Item().PaddingTop(20);

                        column.Item()
                            .Text("EXPERIENCE")
                            .FontSize(18)
                            .Bold();

                        column.Item().Text(
                            "Worked on collaborative backend projects with focus on API architecture and authentication systems.");
                    });
            });
        });

        return document.GeneratePdf();
    }

    private byte[] GenerateMinimalTemplate(User user)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Content()
                    .Column(column =>
                    {
                        column.Item()
                            .Text($"{user.FirstName} {user.LastName}")
                            .FontSize(22);

                        column.Item().Text(user.Email);

                        column.Item().Text(user.PhoneNumber);

                        column.Item().PaddingTop(15);

                        column.Item().Text(
                            "Backend developer focused on ASP.NET Core and PostgreSQL.");

                        column.Item().PaddingTop(15);

                        column.Item().Text("Skills:");

                        column.Item().Text("C#, ASP.NET Core, Docker");
                    });
            });
        });

        return document.GeneratePdf();
    }

    private byte[] GenerateProfessionalTemplate(User user)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);

                page.Content()
                    .Column(column =>
                    {
                        column.Item()
                            .Text($"{user.FirstName} {user.LastName}")
                            .FontSize(30)
                            .Bold();

                        column.Item().Text($"Email: {user.Email}");

                        column.Item().Text($"Phone: {user.PhoneNumber}");

                        column.Item().PaddingTop(25);

                        column.Item()
                            .Text("Professional Summary")
                            .FontSize(20)
                            .Bold();

                        column.Item().Text(
                            "Experienced and motivated software developer with passion for backend systems, APIs, databases and scalable architecture.");

                        column.Item().PaddingTop(25);

                        column.Item()
                            .Text("Technical Skills")
                            .FontSize(20)
                            .Bold();

                        column.Item().Text("• ASP.NET Core Web API");
                        column.Item().Text("• PostgreSQL");
                        column.Item().Text("• Docker");
                        column.Item().Text("• EF Core");
                        column.Item().Text("• REST APIs");
                        column.Item().Text("• Git");

                        column.Item().PaddingTop(25);

                        column.Item()
                            .Text("Education")
                            .FontSize(20)
                            .Bold();

                        column.Item().Text(
                            "Computer Science Student");
                    });
            });
        });

        return document.GeneratePdf();
    }
}