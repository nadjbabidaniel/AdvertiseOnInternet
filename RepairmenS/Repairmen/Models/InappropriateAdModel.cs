using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repairmen.Models
{
    public class InappropriateAdModel
    {
        public System.Guid AdId { get; set; }
        public System.Guid UserId { get; set; }
        public System.Guid Id { get; set; }
        public string Description { get; set; }
    }
}