using Arket.Data;
using Arket.DTOs;
using Arket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Arket.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Arket.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtHelper _jwtHelper;
    
    public AuthController(AppDbContext context, JwtHelper jwtHelper)
    {
        _context = context;
        _jwtHelper = jwtHelper;
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
    [Authorize]
    public async Task<IActionResult> GetUsers() 
    { 
        var users = await _context.Users
            .Select(x => new UserResponseDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber
            })
            .ToListAsync();
        return Ok(users); 
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null)
            return Unauthorized("Invalid email or password");

        var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!passwordValid)
            return Unauthorized("Invalid email or password");

        var token = _jwtHelper.GenerateToken(user);

        return Ok(new
        {
            token,
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email
        });
    }
}