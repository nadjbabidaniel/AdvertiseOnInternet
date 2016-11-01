using RepairmenModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Repairmen.Models
{
    public class RoleModel
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        public System.Guid Id { get; set; }
    }
}