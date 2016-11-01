using RepairmenModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Repairmen.Models
{
    public class UserModel
    {
        [Required]
        [StringLength(30)]
        public string Username { get; set; }

        [Required]
        [MaxLength(128)]     
        public string Password { get; set; }

        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        [Required]
        [StringLength(30)]
        [EmailAddress]
        public string Email { get; set; }

        public System.Guid RoleId { get; set; }

        public System.Guid Id { get; set; }
        public string LoginFlag { get; set; }
        public Nullable<System.DateTime> SignupDate { get; set; }
        public Nullable<bool> Locked { get; set; }
        public Nullable<bool> NotifyEmail { get; set; }
        public Nullable<bool> NotifySMS { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<bool> PasswordChange { get; set; }
    }
}