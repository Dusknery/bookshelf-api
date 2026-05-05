using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtToken _jwt;

    public AuthController(AppDbContext context, JwtToken jwt)
    {
        _context = context;
        _jwt = jwt;
    }


    //sign up
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (existingUser != null)
            return BadRequest(new { error = "Email is already registered" });

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User created", userId = user.Id });
    }

    //login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null)
            return Unauthorized("invalid credentials");

        var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!passwordValid)
            return Unauthorized(new { error = "Invalid credentials" });

        var token = _jwt.CreateToken(user.Id.ToString(), user.Email);

        return Ok(new { token });
    }
}