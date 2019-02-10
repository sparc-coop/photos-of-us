using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;
using Kuvio.Kernel.Core;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Web.Utilities;
using PhotosOfUs.Model.Photos.Commands;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Stripe;
using System;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/RandomPhotos")]
    public class RandomPhotoController : Controller
    {
        private readonly IRepository<Photo> _photos;

        public RandomPhotoController(IRepository<Photo> photoRepository)
        {
            _photos = photoRepository;
        }

        [HttpGet]
        public PhotoViewModel Get()
        {
            var random = new Random();
            var photos = _photos.Where(x => x.PublicProfile && !x.IsDeleted);
            var toSkip = random.Next(0, photos.Count());
            return photos.Skip(toSkip).Take(1).FirstOrDefault().ToViewModel<PhotoViewModel>();
        }
    }
}
