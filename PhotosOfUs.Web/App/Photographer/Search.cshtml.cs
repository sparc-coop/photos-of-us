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
        public List<Tag> Tags { get; private set; }

        public void OnGet()
        {
           Photos = _photos.Where(x => x.PhotographerId == User.ID() && x.PublicProfile && !x.IsDeleted).ToList();
        }

        public void OnPost(string tagnames)
        {
            string[] tagarray = tagnames.Split(' ');
            
            Photos = _photos.Include("PhotoTag.Tag")
                .Where(x => x.PhotographerId == User.ID() && x.PublicProfile && x.PhotoTag.Any(y => tagarray.Contains(y.Tag.Name)))
                .ToList();
            
            Tags = new List<Tag>();
        }
    }
}