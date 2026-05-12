using Arket.Data;
using Arket.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arket.Controllers;

[ApiController]
[Route("api/cv")]
public class CvController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly CvPdfService _cvPdfService;

    public CvController(
        AppDbContext context,
        CvPdfService cvPdfService)
    {
        _context = context;
        _cvPdfService = cvPdfService;
    }

    [HttpGet("pdf/{userId}")]
    public async Task<IActionResult> GeneratePdf(
        int userId,
        string template = "modern")
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        var pdfBytes = _cvPdfService.GenerateCvPdf(
            user,
            template);

        return File(
            pdfBytes,
            "application/pdf",
            $"cv-{template}.pdf");
    }
    
    
}