//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RepairmenModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ad
    {
        public Ad()
        {
            this.Comments = new HashSet<Comment>();
            this.Ratings = new HashSet<Rating>();
            this.InappropriateAds = new HashSet<InappropriateAd>();
        }
    
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public System.Guid CategoryId { get; set; }
        public System.Guid UserId { get; set; }
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> CityId { get; set; }
        public Nullable<decimal> AvgRate { get; set; }
        public Nullable<int> VoteCounter { get; set; }
        public Nullable<int> CommentCounter { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> InappCounter { get; set; }
        public string ImagePath { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string Website { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public Nullable<int> ViewCount { get; set; }
    
        public virtual User User { get; set; }
        public virtual Category Category { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<InappropriateAd> InappropriateAds { get; set; }
    }
}
