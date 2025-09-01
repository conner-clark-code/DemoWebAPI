using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DemoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IConfiguration Config { get; }

        public AuthController(IConfiguration config)
        {
            Config = config;
        }
        public record AuthData(string? UserName, string? Password);
        public record UserData(int Id, string UserName);

        // POST: api/Auth/token
        [HttpPost("token")]
        public ActionResult<string> Authenticate([FromBody] AuthData loginData)
        {
            return ValidateCredentials(loginData);
        }

        private string GenerateToken(UserData user)
        {
            //Use the secret (private) key from configuration to generate the token
            var secretKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(
                    Config.GetValue<string>("Authentication:SecretKey") ?? throw new InvalidOperationException("Secret key not found in configuration")
                )
            );

            //Sign the token using HMAC-SHA256 algorithm
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            //Claims
            List<Claim> claims =
            [
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName)
            ];

            var token = new JwtSecurityToken(
                issuer: Config.GetValue<string>("Authentication:Issuer"),
                audience: Config.GetValue<string>("Authentication:Audience"),
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: signingCredentials
            );
        }

        private UserData ValidateCredentials(AuthData userData)
        {
            if (userData.UserName is null || userData.Password is null)
            {
                throw new ArgumentException("Username or password is null");
            }
            // In a real application, you would validate the credentials against a user store
            if (userData.Password != "password")
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Credentials are valid make token
            return new UserData(new Random().Next(1, 1000), userData.UserName);
        }
    }
}
