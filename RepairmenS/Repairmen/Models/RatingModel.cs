using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repairmen.Models
{
    public class RatingModel
    {

        public System.Guid Id { get; set; }
        public System.Guid AdId { get; set; }
        public System.Guid UserId { get; set; }
        public Nullable<decimal> Value { get; set; }
    }
}