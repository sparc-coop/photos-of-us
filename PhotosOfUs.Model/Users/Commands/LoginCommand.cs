using System.Linq;
using System.Security.Claims;
using Kuvio.Kernel.Core;
using Kuvio.Kernel.Auth;
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

        public User Execute(ClaimsPrincipal principal) 
        {
            var azureId = principal.AzureID();

            //var user = Set.Find(x => x.UserIdentities.Any(y => y.AzureID == azureId)); // User with identity
            var user = _users.Query.FirstOrDefault(x => x.UserIdentities.Any(y => y.AzureID == azureId));

            if (user == null)
            {
                user = _users.Query.FirstOrDefault(x => x.Email == principal.Email()); // User without identity
                if (user == null)
                {
                    user = new User(principal.DisplayName(), principal.Email(), principal.AzureID(), principal.HasClaim("tfp", "B2C_1_SiUpOrIn_Photographer"));
                    user = _users.Add(user);
                }
            }

            _users.Execute(user.Id, u => u.Login(principal, principal.AzureID()));

            return user;
        }
    }
}