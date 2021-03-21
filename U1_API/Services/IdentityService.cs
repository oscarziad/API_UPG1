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

namespace U1_API.Services
{
    public class IdentityService : IIdentityService
    {

        private readonly SqlDbContext _context;
        private IConfiguration _configuration { get; }

        public IdentityService(SqlDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public async Task<bool> CreateUserAsync(RegisterUser model)
        {
            if (!_context.Users.Any(user => user.Email == model.Email))
            {
                try
                {
                    var user = new User()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email
                    };
                    user.CreatePasswordWithHash(model.Password);
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    return true;
                }
                catch { }
            }

            return false;
        }


        public async Task<LogInResponse> LogInAsync(string email, string password)
        {

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);

                if (user != null)
                {
                    try
                    {
                        if (user.ValidatePasswordHash(password))
                        {
                            var tokenHandler = new JwtSecurityTokenHandler();
                            var _secretKey = Encoding.UTF8.GetBytes(_configuration.GetSection("SecretKey").Value);
                            if (_secretKey != null)
                            {
                                var tokenDescriptor = new SecurityTokenDescriptor
                                {
                                    Subject = new ClaimsIdentity(new Claim[] { new Claim("UserId", user.Id.ToString()) }),
                                    Expires = DateTime.Now.AddHours(1),
                                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secretKey), SecurityAlgorithms.HmacSha512Signature)
                                };
                                if(tokenDescriptor != null)
                                {
                                    var _accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
                                    if (_accessToken != null)
                                    {
                                        _context.SessionTokens.Add(new SessionToken { UserId = user.Id, AccessToken = _accessToken });
                                        await _context.SaveChangesAsync();

                                        return new LogInResponse
                                        {
                                            Succeeded = true,
                                            Result = new LogInResponseResult
                                            {
                                                Id = user.Id,
                                                Email = user.Email,
                                                AccessToken = _accessToken
                                            }
                                        };
                                    }
                                }
                                

                            }

                        }

                    }
                    catch { }
                }
            }
            catch { }

            return new LogInResponse { Succeeded = false };

        }

        public async Task<IEnumerable<UserResponse>> GetUsersAsync()
        {
            var users = new List<UserResponse>();

            foreach (var user in await _context.Users.ToListAsync())
            {
                users.Add(new UserResponse { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email });
            }

            return users;
        }



        public async Task<IEnumerable<IssueResponse>> GetIssuesAsync()
        {
            var issues = new List<IssueResponse>();

            if (_context.Issues.Any())
            {
                foreach (var issue in await _context.Issues.ToListAsync())
                {
                    issues.Add(new IssueResponse { CustomerId = issue.CustomerId, UserId = issue.UserId, IssueDate = issue.IssueDate, UpdateDate = issue.UpdateDate, IssueStatus = issue.IssueStatus });
                }

                return issues;
            }

            return null;
        }


        public bool ValidateAccessRights(RequestUser requestUser)
        {
            if (_context.SessionTokens.Any(x => x.UserId == requestUser.UserId && x.AccessToken == requestUser.AccessToken))
                return true;

            return false;
        }


        public async Task<bool> CreateIssueAsync(RegisterIssue model)
        {
                try
                {
                    var issue = new Issue()
                    {
                        UserId = model.UserId,
                        CustomerId = model.CustomerId,
                        IssueDate = model.IssueDate,
                        UpdateDate = model.UpdateDate,
                        IssueStatus = model.IssueStatus
                    };

                    _context.Issues.Add(issue);
                    await _context.SaveChangesAsync();

                    return true;
                }
                catch { }

            return false;
        }


        public async Task<IEnumerable<IssueResponse>> GetIssuesAsyncStatus(string status)
        {
            var issues = new List<IssueResponse>();

            if (_context.Issues.Any())
            {
                foreach (var issue in await _context.Issues.ToListAsync())
                {
                    if (issue.IssueStatus == status)
                    {
                        issues.Add(new IssueResponse { CustomerId = issue.CustomerId, UserId = issue.UserId, IssueDate = issue.IssueDate, UpdateDate = issue.UpdateDate, IssueStatus = issue.IssueStatus });
                    }
                }
                if (issues != null)
                {
                    return issues;
                }

            }

            return null;
        }

    }
}
