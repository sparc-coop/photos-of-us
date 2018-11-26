using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;
using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Web.Utilities;
using PhotosOfUs.Model.Photos.Commands;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Stripe;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Photo")]
    public class PhotoApiController : Controller
    {
        private readonly IRepository<Photo> _photos;
        private readonly IRepository<Tag> _tag;

        public PhotoApiController(IRepository<Photo> photoRepository, IRepository<Tag> tagRepository)
        {
            _photos = photoRepository;
            _tag = tagRepository;
        }

        [HttpGet]
        [Route("{photoId:int}")]
        public PhotoViewModel GetPhoto(int photoId)
        {
            return _photos.Find(x => x.Id == photoId).ToViewModel<PhotoViewModel>();
        }

        [HttpGet]
        [Route("GetPublicIds")]
        public List<int> GetPublicIds()
        {
            var photos = _photos.Where(x => x.PublicProfile && !x.IsDeleted).ToList();

            List<int> photoIds = new List<int>();
            foreach (var photo in photos)
            {
                photoIds.Add(photo.Id);
            }

            return photoIds;
        }

        [HttpGet]
        [Route("GetCodePhotos/{code}")]
        public List<PhotoViewModel> GetCodePhotos(string code)
        {
            return _photos.Where(x => x.Code == code).ToViewModel<PhotoViewModel>().ToList();
        }

        [HttpGet]
        [Route("GetPrintTypes")]
        public List<PrintTypeViewModel> GetPrintTypes()
        {
            return new Photo().GetPrintTypes().ToList().ToViewModel<List<PrintTypeViewModel>>();
        }

        [HttpGet]
        [Route("GetAllTags")]
        public List<TagViewModel> GetAllTags()
        {
            return _tag.ToViewModel<List<TagViewModel>>();
        }

        public ActionResult Purchase(int id)
        {
            var photo = _photos.Find(x => x.Id == id);
            var viewModel = photo.ToViewModel<PhotoViewModel>();

            return View(viewModel);
        }
    }
}
