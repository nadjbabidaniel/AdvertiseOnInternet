﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repairmen.Models
{
    public class CommentVoteModel
    {
        public System.Guid Id { get; set; }

        public bool Vote { get; set; }

        //[Required]
        public System.Guid UserId { get; set; }

        //[Required]
        public System.Guid CommentId { get; set; }

    }
}

