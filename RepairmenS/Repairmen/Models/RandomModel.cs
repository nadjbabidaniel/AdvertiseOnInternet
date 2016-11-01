using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using RepairmenModel;

namespace Repairmen.Models
{
    public class RandomModel
    {
        public System.Guid Id { get; set; }
        public string Username { get; set; }
        public string RandomString { get; set; }
    }
}