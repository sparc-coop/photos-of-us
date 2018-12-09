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
    public class SearchModel : PageModel
    {
        private readonly IRepository<Photo> _photos;

        public SearchModel(IRepository<Photo> photos)
        {
            _photos = photos;
        }

        public List<Photo> Photos { get; private set; }

        public void OnGet()
        {
           Photos = _photos.Where(x => x.PhotographerId == User.ID() && x.PublicProfile && !x.IsDeleted).ToList();
        }
    }
}