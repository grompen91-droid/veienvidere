using Arket.Data;
using Arket.DTOs;
using Arket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arket.Controllers;

[ApiController]
[Route("api/cv")]
public class CvController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly CvPdfService _cvPdfService;

    public CvController(AppDbContext context, CvPdfService cvPdfService)
    {
        _context = context;
        _cvPdfService = cvPdfService;
    }

    [HttpPost("pdf/{userId}")]
    [Authorize]
    public async Task<IActionResult> GeneratePdf(
        int userId,
        [FromBody] GenerateCvDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return NotFound();

        var pdfBytes = _cvPdfService.GenerateCvPdf(user, dto);

        return File(pdfBytes, "application/pdf", "cv.pdf");
    }
}