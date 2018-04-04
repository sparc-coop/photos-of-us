using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.Models
{
    public partial class UserIdentity
    {
        public string AzureID { get; set; }
        public string IdentityProvider { get; set; }
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public User User { get; set; }
    }
}
