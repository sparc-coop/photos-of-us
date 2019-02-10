using System.Threading.Tasks;
using Kuvio.Kernel.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Web.Utilities;

namespace PhotosOfUs.Pages.Events
{
    public class PurchaseModel : PageModel
    {
        private readonly IRepository<Photo> _photos;

        public Photo Photo { get; set; }
        public User Photographer { get; set; }

        public PurchaseModel(IRepository<Photo> photos)
        {
            _photos = photos;
        }

        public IActionResult OnGet(int eventId, int photoId)
        {
            Photo = _photos.Find(photoId);
            if (Photo?.EventId != eventId) return NotFound();
            Photographer = Photo.Photographer;
            return Page();
        }

    }
}