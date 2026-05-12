using Arket.Data;
using Arket.DTOs;
using Arket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Arket.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    public AuthController(AppDbContext context)
    {
        _context = context;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var userExists = await _context.Users
            .AnyAsync(x => x.Email == dto.Email);
        if (userExists)
        {
            return BadRequest("User already exists");
        }
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("User created");
    }
    
    [HttpGet("users")] 
    public async Task<IActionResult> GetUsers() 
    { 
        var users = await _context.Users.ToListAsync();
        return Ok(users); 
    }
}