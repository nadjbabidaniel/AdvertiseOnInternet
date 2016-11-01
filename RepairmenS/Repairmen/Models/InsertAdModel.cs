using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Repairmen.Models
{
    public class InsertAdModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(300)]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Location { get; set; }

        [Required]
        [StringLength(30)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(30)]
        [EmailAddress]
        public string Email { get; set; }

        public string longitude { get; set; }

        public string latitude { get; set; }

        public string Website { get; set; }

        public System.Guid Id { get; set; }

        public CategoryModel Category { get; set; }

        public CityModel City { get; set; }

        public string Image { get; set; }

        public Nullable<bool> IsPaid { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public Nullable<int> ViewCount { get; set; }
        
    }
}