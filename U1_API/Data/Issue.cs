using System;
using System.Collections.Generic;

#nullable disable

namespace U1_API.Data
{
    public partial class Issue
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string IssueStatus { get; set; }
    }
}
