using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotosOfUs.Model.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace PhotosOfUs.Pages.Photographer
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly IRepository<User> _users;

        public ProfileModel(IRepository<User> users)
        {
            _users = users;
        }

        public User Photographer { get; private set; }
        public bool IsAdmin { get; private set; }

        public IActionResult OnGet(int? userId = null)
        {
           if (userId == null) userId = User.ID();

           Photographer = _users.Find(x => x.Id == userId);
           if (Photographer == null) return NotFound();
           
           if (Photographer.Id == User.ID()) IsAdmin = true;
           
           return Page();
        }
    }
}