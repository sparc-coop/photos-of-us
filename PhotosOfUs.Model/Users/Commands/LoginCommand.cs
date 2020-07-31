using System.Linq;
using System.Security.Claims;
using Kuvio.Kernel.Core;

namespace PhotosOfUs.Core.Users.Commands
{
    public class LoginCommand
    {
        private readonly IDbRepository<User> _usersRepository;

        public LoginCommand(IDbRepository<User> usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public User Execute(ClaimsPrincipal principal, string azureId, string email, string firstName, string lastName, bool isPhotographer)
        {
            var user = _usersRepository.Query.Where(x => x.UserIdentities.Any(y => y.AzureID == azureId)).FirstOrDefault();

            if (user == null)
            {
                user = _usersRepository.Query.Where(x => x.Email.ToLower() == email.ToLower()).FirstOrDefault(); // User without identity

                if (user == null)
                {
                    Role role = Role.Customer;
                    if (isPhotographer)
                    {
                        role = Role.Photographer;
                    }


                    user = new User(firstName, lastName, email, role, azureId);
                    user = _usersRepository.AddAsync(user).Result;
                    
                }
            }

            _usersRepository.ExecuteAsync(user, u => u.Login(principal, azureId));
            _usersRepository.CommitAsync();

            return user;
        }
    }
}