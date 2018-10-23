using System.Linq;
using System.Security.Claims;
using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Model
{
    public class LoginCommand : Command<User>
    {
        //private IRepository<User> Users;

        public LoginCommand(IRepository<User> repository) : base(repository)
        {

        }

        public User Execute(ClaimsPrincipal principal) 
        {
            var user = Set.Find(x => x.UserIdentities.Any(y => y.AzureID == principal.AzureID())); // User with identity
            if (user == null)
            {
                user = Set.Find(x => x.Email == principal.Email()); // User without identity
                if (user == null)
                {
                    user = new User(principal.DisplayName(), principal.Email(), principal.AzureID(), principal.HasClaim("tfp", "B2C_1_SiUpOrIn_Photographer"));
                    user = Set.Add(user);
                }
            }

            user.Login(principal, principal.AzureID());

            return user;
        }
    }
}