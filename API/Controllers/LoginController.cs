using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Config;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;
[ApiController]
[Route("[controller]")]
public class LoginController : Controller
{
    private JwtConfig _config;

    public LoginController(IOptions<JwtConfig> config)
    {
        _config = config.Value;
    }
    // GET
    [HttpPost]
    public IActionResult GenerateToken([FromBody] Users user)
    {
        if (user.Login != "admin" && user.Password != "admin")
        {
            return Unauthorized();
        }
        //var mySecret = "3503C73BDD6A47838E8EA23E89C3C9E2";
        var mySecret = _config.Key;
        var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

        var myIssuer = "http://localhost:5049/";
        var myAudience = "hhttp://localhost:5049/";

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, 1.ToString()),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = myIssuer,
            Audience = myAudience,
            SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(tokenHandler.WriteToken(token));
    }
}