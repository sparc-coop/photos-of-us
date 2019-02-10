using Kuvio.Kernel.Core;
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
    public class AccountModel : PageModel
    {
        private readonly IRepository<User> _users;

        public AccountModel(IRepository<User> users)
        {
            _users = users;
        }

        public User Photographer { get; private set; }

        public IActionResult OnGet()
        {
           Photographer = _users.Find(x => x.Id == User.ID());
           if (Photographer == null) return NotFound();
           
           return Page();
        }

        public IActionResult OnPost(User user)
        {
            var photographer = _users.Find(x => x.Id == User.ID());
            photographer.UpdateProfile(user.Email, user.FirstName, user.LastName, user.DisplayName, user.JobPosition, user.ProfilePhotoUrl, user.Bio);
            _users.Commit();
            
            return Page();
        }
    }
}