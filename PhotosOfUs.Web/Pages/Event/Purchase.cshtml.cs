using System.Threading.Tasks;
using Kuvio.Kernel.Architecture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Web.Utilities;

namespace PhotosOfUs.Pages.Event
{
    public class PurchaseModel : PageModel
    {
        private IRepository<Photo> _photos;

        [BindProperty]
        public PhotoViewModel Photo { get; set; }

        public PurchaseModel(IRepository<Photo> photos)
        {
            _photos = photos;
        }

        public void OnGet(int id)
        {
            var photo = _photos.Find(x => x.Id == id);
            Photo = photo.ToViewModel<PhotoViewModel>();
        }

    }
}