using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using U1_API.Data;
using U1_API.Models;
using U1_API.Services;

namespace U1_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IIdentityService _identity;
        private readonly SqlDbContext _context;
        private IConfiguration _configuration { get; }

        public UsersController(SqlDbContext context, IIdentityService identity, IConfiguration configuration)
        {
            _context = context;
            _identity = identity;
            _configuration = configuration;
        }


        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] RegisterUser model)
        {
            if (await _identity.CreateUserAsync(model))
                return new OkResult();

            return new BadRequestResult();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> SignIn([FromBody] LogIn model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null)
                {
                    if (user.ValidatePasswordHash(model.Password))
                    {
                        var th = new JwtSecurityTokenHandler();
                        var expiresDate = DateTime.Now.AddDays(1);

                        var td = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim("UserId", user.Id.ToString()),
                                new Claim("Expires", expiresDate.ToString())
                            }),
                            Expires = expiresDate,
                            SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(_configuration.GetSection("SecretKey").Value)),
                                    SecurityAlgorithms.HmacSha512Signature
                                )
                        };

                        var _accessToken = th.WriteToken(th.CreateToken(td));

                        return new OkObjectResult(_accessToken);
                    }
                }
            }
            catch { return new BadRequestObjectResult("nologin"); }

            return new BadRequestObjectResult("nologin");
        }
    }

}
