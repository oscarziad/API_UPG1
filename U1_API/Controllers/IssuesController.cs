using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using U1_API.Data;
using U1_API.Models;

namespace U1_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly SqlDbContext _context;

        public IssuesController(SqlDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterIssue model)
        {
            try
            {
                var issue = new Issue()
                {
                    CustomerId = model.CustomerId,
                    UserId = model.UserId,
                    IssueDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    IssueStatus = model.IssueStatus
                };

                _context.Issues.Add(issue);
                await _context.SaveChangesAsync();

            }
            catch
            {
                return new BadRequestResult();
            }
            return new OkResult();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Issue>>> GetIssues()
        {
            var issues = new List<Issue>();
            var issuesSorted = new List<Issue>();


            if (_context.Issues.Any())
            {
                foreach (var issue in await _context.Issues.ToListAsync())
                {
                    issues.Add(new Issue { Id =issue.Id, CustomerId = issue.CustomerId, UserId = issue.UserId, IssueDate = issue.IssueDate, UpdateDate = issue.UpdateDate, IssueStatus = issue.IssueStatus });
                }
                if (issues != null)
                {
                    issuesSorted = issues.OrderBy(o => o.CustomerId).ToList();
                    return issuesSorted;
                }

            }

            return null;
        }


        [HttpGet("status")]
        public async Task<ActionResult<IEnumerable<Issue>>> GetStatus(string i)
        {
            var issues = new List<Issue>();

            if (_context.Issues.Any())
            {
                foreach (var issue in await _context.Issues.ToListAsync())
                {
                    if (issue.IssueStatus == i)
                    {
                        issues.Add(new Issue { Id = issue.Id, CustomerId = issue.CustomerId, UserId = issue.UserId, IssueDate = issue.IssueDate, UpdateDate = issue.UpdateDate, IssueStatus = issue.IssueStatus });
                    }
                }
                if (issues != null)
                {
                    return issues;
                }

            }

            return null;

        }

        [HttpGet("customerid")]
        public async Task<ActionResult<IEnumerable<Issue>>> GetCustomerId(int i)
        {
            var issues = new List<Issue>();

            if (_context.Issues.Any())
            {
                foreach (var issue in await _context.Issues.ToListAsync())
                {
                    if (issue.CustomerId == i)
                    {
                        issues.Add(new Issue { Id = issue.Id, CustomerId = issue.CustomerId, UserId = issue.UserId, IssueDate = issue.IssueDate, UpdateDate = issue.UpdateDate, IssueStatus = issue.IssueStatus });
                    }
                }
                if (issues != null)
                {
                    return issues;
                }

            }

            return null;

        }

        [HttpGet("dates")]
        public async Task<ActionResult<IEnumerable<Issue>>> GetDates()
        {
            var issues = new List<Issue>();
            

            if (_context.Issues.Any())
            {
                foreach (var issue in await _context.Issues.ToListAsync())
                {
                    issues.Add(new Issue { Id = issue.Id, CustomerId = issue.CustomerId, UserId = issue.UserId, IssueDate = issue.IssueDate, UpdateDate = issue.UpdateDate, IssueStatus = issue.IssueStatus });
                }
                if (issues != null)
                {
                    issues.Sort((x, y) => DateTime.Compare(x.IssueDate, y.IssueDate));
                    return issues;
                }

            }

            return null;

        }

        [HttpPut("update")]
        public async Task<IActionResult> PutIssue([FromBody] Issue issue, int id)
        {
            if (id != issue.Id)
                return BadRequest();

            _context.Entry(issue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
                
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueExists(id))
                    return NotFound();

                else
                    throw;
            }
            return NoContent();
        }

        private bool IssueExists(int id)
        {
            return _context.Issues.Any(e => e.Id == id);
        }

    }
}
