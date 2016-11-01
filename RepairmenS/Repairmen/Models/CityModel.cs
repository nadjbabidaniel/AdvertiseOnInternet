using RepairmenModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Repairmen.Models
{
    public class CityModel
    {
        public System.Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string CountryName { get; set; }
        [Required]
        [MaxLength(200)]
        public string CityName { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}