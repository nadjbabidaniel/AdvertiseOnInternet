using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Repairmen.Models
{
    public class InappropriateCommentModel
    {
        public System.Guid CommentId { get; set; }
        public System.Guid UserId { get; set; }
        public System.Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }
    }
}