using System.Security.Claims;
using System.Threading.Tasks;
using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs
{
    public class NavViewComponent : ViewComponent
    {
        private readonly IRepository<User> _users;
        public User CurrentUser { get; set; }

        public NavViewComponent(IRepository<User> users)
        {
            _users = users;
        }

        public IViewComponentResult Invoke()
        {
            User currentUser = null;
            var user = User as ClaimsPrincipal;
            if (user != null)
                CurrentUser = _users.Find(x => x.Id == user.ID());

            return View(currentUser);
        }
    }
}