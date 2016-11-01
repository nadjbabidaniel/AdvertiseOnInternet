using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Repairmen.Models
{
    public class CommentModel
    {
        
        [StringLength(200)]
       // [Required]
        public string Text { get; set; }

        public short PositiveVote { get; set; }

        public short NegativeVote { get; set; }

        public short Counter { get; set; }

        public System.Guid Id { get; set; }

        //[Required]
        public System.Guid UserId { get; set; }

        //[Required]
        public System.Guid AdId { get; set; }

        public string Username { get; set; }

        public Nullable<bool> Approved { get; set; }

        public Nullable<bool> Delete { get; set; }

        public Nullable<System.DateTime> Date { get; set; }
    }
}