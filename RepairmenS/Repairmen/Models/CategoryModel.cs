using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using RepairmenModel;
namespace Repairmen.Models
{
    public class CategoryModel
    {
        [Required]
        [StringLength(30)]
        public string CatName { get; set; }

        public Nullable<bool> Approved { get; set; }

        public Nullable<bool> Delete { get; set; } 

        public System.Guid Id { get; set; }
    }
}