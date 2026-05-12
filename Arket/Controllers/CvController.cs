using Arket.Data;
using Arket.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Bedrifts_backend.Controllers;
[ApiController]
[Route("api/cv")]
public class CvController : ControllerBase
{
    private readonly AppDbContext _context;
    public CvController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet("generate/{userId}")]
    public async Task<IActionResult> GenerateCv(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            return NotFound();
        }
        var cv = $@"
FULL NAME:
{user.FirstName} {user.LastName}
EMAIL:
{user.Email}
PHONE:
{user.PhoneNumber}
PROFILE:
Motivated and ambitious developer with strong interest in backend
development, ASP.NET Core, PostgreSQL, Docker and modern web technologies.
Passionate about solving technical problems and building scalable systems.
SKILLS:
- C#
- ASP.NET Core Web API
- PostgreSQL
- Docker
- Entity Framework Core
- Git
- REST API
EXPERIENCE:
Worked on collaborative backend projects with focus on API architecture,
authentication systems and database design.
EDUCATION:
Computer Science Student
LANGUAGES:
- English, Norwegian
";
        return Ok(cv);
    }
}