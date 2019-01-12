using Kuvio.Kernel.Architecture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotosOfUs.Model.Models;
using Kuvio.Kernel.Auth;
using Microsoft.AspNetCore.Authorization;

namespace PhotosOfUs.Pages.Events
{
    [Authorize]
    public class AdminIndexModel : PageModel
    {
        private IRepository<User> _users;

        public string Name { get; set; }
        public int PhotographerId { get; set; }

        public AdminIndexModel(IRepository<User> users)
        {
            _users = users;
        }

        public IActionResult OnGet()
        {
            var photographer = _users.Find(x => x.Id == User.ID());

            if (photographer.IsPhotographer == true)
            {
                PhotographerId = photographer.Id;
                Name = User.Identity.Name;
                return Page();
            }
            else
            {
                return Redirect("/Photographer/Search");
            }
        }
    }
}