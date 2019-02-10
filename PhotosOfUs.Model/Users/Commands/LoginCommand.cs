using System.Linq;
using System.Security.Claims;
using Kuvio.Kernel.Core;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Model
{
    public class LoginCommand
    {
        private readonly IRepository<User> _users;

        public LoginCommand(IRepository<User> users)
        {
            _users = users;
        }

        public User Execute(ClaimsPrincipal principal, string azureId, string email, string displayName, bool isPhotographer) 
        {
            var user = _users.Query.FirstOrDefault(x => x.UserIdentities.Any(y => y.AzureID == azureId));

            if (user == null)
            {
                user = _users.Query.FirstOrDefault(x => x.Email == email); // User without identity
                if (user == null)
                {
                    user = new User(displayName, email, azureId, isPhotographer);
                    user = _users.Add(user);
                }
            }

            _users.Execute(user.Id, u => u.Login(principal, azureId));

            return user;
        }
    }
}