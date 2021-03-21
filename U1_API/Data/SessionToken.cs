using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace U1_API.Data
{
    public partial class SessionToken
    {
        //public int UserId { get; set; }

        ////bytte från byte[] till string (?)
        //public string AccessToken { get; set; }

        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "varbinary(max)")]
        public string AccessToken { get; set; }
    }
}
