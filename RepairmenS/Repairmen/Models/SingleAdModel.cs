using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using RepairmenModel;

namespace Repairmen.Models
{
    public class SingleAdModel
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

        [Range(0, 5)]
        public decimal AvgRate { get; set; }

        [Range(0, 200)]
        public int VoteCounter { get; set; }

        public System.Guid Id { get; set; }

        public string Category { get; set; }

        [Required]
        public System.Guid UserId { get; set; }

        public string City { get; set; }

        public int CommentCounter { get; set; }

        public System.DateTime Date { get; set; }

        public int InappCounter { get; set; }

        public string ImagePath { get; set; }

        public string longitude { get; set; }

        public string latitude { get; set; }

        public string Website { get; set; }

        public Nullable<bool> IsPaid { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public Nullable<int> ViewCount { get; set; }

        public Nullable<bool> Approved { get; set; }

        public Nullable<bool> Delete { get; set; }
    }
}