using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PhotosOfUs.Model.Models
{
    public class PhotosOfUsPrincipal : ClaimsPrincipal
    {
        public User User { get; }

        public UserIdentity UserIdentity { get; }
        private PhotosOfUsContext Context { get; }

        public PhotosOfUsPrincipal(ClaimsPrincipal principal, User user = null) : base(principal)
        {
            Context = new PhotosOfUsContext();
            if (user != null)
            {
                User = user;
            }
            else
            {
                UserIdentity = Context.UserIdentity.Include(x => x.User).FirstOrDefault(x => x.AzureID == AzureID);
                User = UserIdentity?.User;
            }

            if (User == null) return;
        }

        public string DisplayName => Get("name") ?? Get("http://schemas.microsoft.com/identity/claims/givenname");
        public string Email => Get("emails");
        public string AzureID => Get("http://schemas.microsoft.com/identity/claims/objectidentifier");

        public static async Task Create(ClaimsIdentity identity)
        {
            var user = new PhotosOfUsPrincipal(new ClaimsPrincipal(identity));
            var context = new PhotosOfUsContext();

            if (user.UserIdentity == null)
            {
                // See if the base user exists
                var baseUser = context.User.FirstOrDefault(x => x.Email == user.Email);
                if (baseUser == null)
                {
                    baseUser = new User
                    {
                        CreateDate = DateTime.UtcNow,
                        DisplayName = user.DisplayName,
                        Email = user.Email,
                    };
                    context.User.Add(baseUser);
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                var newIdentity = new UserIdentity
                {
                    AzureID = user.AzureID,
                    CreateDate = DateTime.UtcNow,
                    LastLoginDate = DateTime.UtcNow,
                    UserID = baseUser.Id
                };
                context.UserIdentity.Add(newIdentity);
                context.SaveChanges();
                user = new PhotosOfUsPrincipal(new ClaimsPrincipal(identity));
            }

            await user.Login();
            identity.AddClaim(new Claim("userID", user.User.Id.ToString()));
            identity.AddClaim(new Claim("userName", user.User.DisplayName));
        }

        private async Task Login()
        {
            if (User == null) return;
            UserIdentity.LastLoginDate = DateTime.UtcNow;
            await Context.SaveChangesAsync();
            Thread.CurrentPrincipal = this;
        }

        private string Get(string claimName)
        {
            return FindFirst(claimName)?.Value;
        }
    }
}
