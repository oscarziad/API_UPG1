using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace U1_API.Models
{
    public class RegisterIssue
    {
        public int CustomerId { get; set; }

        public int UserId { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string IssueStatus { get; set; }

    }
}
