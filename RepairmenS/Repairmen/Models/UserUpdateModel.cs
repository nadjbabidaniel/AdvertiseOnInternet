using RepairmenModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Repairmen.Models
{
    public class UserUpdateModel
    {        
        
        [MaxLength(128)]     
        public string OldPassword { get; set; }        
        [MaxLength(128)]
        public string NewPassword { get; set; }
       
        [StringLength(30)]
        public string Username { get; set; }

        public string DisplayName { get; set; }

        public Nullable<bool> NotifyEmail { get; set; }
        public Nullable<bool> NotifySMS { get; set; }
        public string PhoneNumber { get; set; }

        [Required]
        public System.Guid UserId { get; set; }
    }
}