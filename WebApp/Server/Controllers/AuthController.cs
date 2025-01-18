using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlazorApp.Shared.DTO;

namespace BlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(DataContext context, IConfiguration configuration) : ControllerBase
    {
        [HttpPost("ValidateJWT")]
        public async Task<ActionResult<string>> ValidateJWT()
        {
            string jwtToken = await new StreamReader(Request.Body).ReadToEndAsync();
            jwtToken = jwtToken.Replace(@"\u0022", "").Replace("\"", "");
            var mysecret = configuration.GetSection("AppSettings:Token").Value;
            var mysecurity_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mysecret));
            var myIssuer = "MyApp";
            var myAudience = "MyApp";
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mysecurity_key
                }, out SecurityToken validatedToken);
            }
            catch(Exception ex)
            {
                return "Invalid";
            }

            return "Valid";
        }

        [HttpPost]  
        public async Task<ActionResult<string>> Login(UserLogin loginRequest)
        {

            var user = await context.Users.FirstOrDefaultAsync(x => (x.Username == loginRequest.Username && x.Password==loginRequest.Password));
            if (user == null)
            {
                return NotFound("No User");
            }
            else
            {
               return Ok(CreateToken(user));
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
             {
                 new Claim(ClaimTypes.Name,user.Username),
                 new Claim(ClaimTypes.Role,user.Role),
             };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key,SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: "MyApp",
                audience: "MyApp",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials:cred
                ) ;

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
