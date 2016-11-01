using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using RepairmenModel;

namespace Repairmen.Models
{
    public class AdModel
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

        public System.Guid CategoryId { get; set; }

        [Required]
        public System.Guid UserId { get; set; }

        public System.Guid CityId { get; set; }

        public int CommentCounter { get; set; }

        public System.DateTime Date { get; set; }

        public int InappCounter { get; set; }

        public string ImagePath { get; set; }

        public string longitude { get; set; }

        public string latitude { get; set; }

        public string Website { get; set; }

        public Nullable<bool> Approved { get; set; }

        public Nullable<bool> Delete { get; set; }

        public string CategoryName { get; set; }

        public string CityName { get; set; }

        public bool? IsPaid { get; set; }

        public DateTime? PaidDate { get; set; }

        public int? ViewCount { get; set; }

        public int PaidDaysLeft { get; set; }

        public int PaidViewsLeft { get; set; }
    }

    public class AdsResult
    {
        public IEnumerable<AdModel> adModel { get; set; }

        public int numberOfResults { get; set; }

        public IEnumerable<AdModel> paidAds { get; set; }
    }
}