using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using U1_API.Models;

namespace U1_API.Services
{
    public interface IIdentityService
    {
        Task<bool> CreateUserAsync(RegisterUser model);
        Task<LogInResponse> LogInAsync(string email, string password);
        Task<IEnumerable<UserResponse>> GetUsersAsync();
        bool ValidateAccessRights(RequestUser requestUser);

        public Task<bool> CreateIssueAsync(RegisterIssue model);
        public Task<IEnumerable<IssueResponse>> GetIssuesAsync();

        public Task<IEnumerable<IssueResponse>> GetIssuesAsyncStatus(string status);


    }
}
