using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtHandler _jwtHandler;

    public AccountController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        JwtHandler jwtHandler)
    {
        _context = context;
        _userManager = userManager;
        _jwtHandler = jwtHandler;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(ApiLoginRequest loginRequest)
    {
        var user = await _userManager.FindByNameAsync(loginRequest.Email);
        
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            return Unauthorized(new ApiLoginResult()
            {
                Success = false,
                Message = "Invalid Email or Password."
            });
        }

        var secToken = await _jwtHandler.GetTokenAsync(user);
        var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
        
        return Ok(new ApiLoginResult()
        {
            Success = true,
            Message = "Login successful",
            Token = jwt
        });
    }
}
