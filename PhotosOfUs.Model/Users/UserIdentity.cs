using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Core.Users
{
    public partial class UserIdentity
    {
        public UserIdentity()
        {

        }

        public UserIdentity(User user, string externalUserId, string identityProvider)
        {
            AzureID = externalUserId;
            IdentityProvider = identityProvider;
            CreateDateUtc = DateTime.UtcNow;
            UserID = user.Id;
        }

        public string AzureID { get; protected set; }
        public string IdentityProvider { get; protected set; }
        public int UserID { get; protected set; }
        public DateTime CreateDateUtc { get; protected set; }
        public DateTime? LastLoginDateUtc { get; set; }

        public User User { get; protected set; }
    }
}
