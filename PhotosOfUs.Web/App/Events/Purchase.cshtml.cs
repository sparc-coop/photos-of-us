using System.Threading.Tasks;
using Kuvio.Kernel.Architecture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Web.Utilities;

namespace PhotosOfUs.Pages.Events
{
    public class PurchaseModel : PageModel
    {
        private IRepository<Photo> _photos;

        public Photo Photo { get; set; }
        public User Photographer { get; set; }

        public PurchaseModel(IRepository<Photo> photos)
        {
            _photos = photos;
        }

        public void OnGet(int id)
        {
            Photo = _photos.Include(x => x.Photographer).Find(x => x.Id == id);
            Photographer = Photo.Photographer;
        }

    }
}