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
            var user = User as ClaimsPrincipal;
            if (user != null)
                currentUser = _users.Find(x => x.Id == user.ID());

            return View(currentUser);
        }
    }
}