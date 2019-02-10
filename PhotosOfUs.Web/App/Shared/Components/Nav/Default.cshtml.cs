using System.Security.Claims;
using System.Threading.Tasks;
using Kuvio.Kernel.Core;
using Kuvio.Kernel.Auth;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs
{
    public class NavViewComponent : ViewComponent
    {
        private readonly IRepository<User> _users;

        public NavViewComponent(IRepository<User> users)
        {
            _users = users;
        }

        public IViewComponentResult Invoke()
        {
            User currentUser = null;
            if (User is ClaimsPrincipal user)
                currentUser = _users.Find(user.ID());

            return View(currentUser);
        }
    }
}