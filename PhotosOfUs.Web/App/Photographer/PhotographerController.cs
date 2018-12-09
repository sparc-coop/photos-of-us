using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Web.Utilities;
using Rotativa.NetCore;
using Rotativa.NetCore.Options;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System;
using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Model.Photos.Commands;


namespace PhotosOfUs.Web.Controllers
{
    [Authorize]
    [Route("/api/Photographer")]
    public class PhotographerController : Controller
    {
        private PhotosOfUsContext _context;
        private readonly IRepository<Photo> _photo;
        private readonly IRepository<User> _user;
        private readonly IRepository<Card> _card;

        public PhotographerController(PhotosOfUsContext context, IRepository<Photo> photoRepository, 
        IRepository<User> userRepository, IRepository<Card> cardRepository)
        {
            _context = context;
            _photo = photoRepository;
            _user = userRepository;
            _card = cardRepository;
        }

        [Authorize]
        [HttpPost]
        [Route("ProfilePhoto")]
        public async Task<UploadPhotoCommand.UploadPhotoCommandResult> UploadProfilePhotoAsync(IFormFile file, [FromServices]UploadPhotoCommand command)
        {
            return await command.ExecuteAsync(User.ID(), file.FileName, file.OpenReadStream());
        }

        public ActionResult Profile(int userId)
        {
            var newId = userId;
            if (userId == 0)
            {
                userId = _user.Find(x => x.AzureId == User.AzureID()).Id;
            }

            var photographer = _user.Find(x => x.Id == userId);
            var photos = _photo.Where(x => x.PublicProfile && !x.IsDeleted && x.PhotographerId == userId).ToList();

            return View(ProfileViewModel.ToViewModel(photos,photographer));
        }

        public ActionResult SalesHistory()
        {
            return View();
        }

        public ActionResult Search()
        {
            var photos = _photo.Where(x => x.PublicProfile && !x.IsDeleted).ToList();

            return View(photos.ToViewModel<List<PhotoViewModel>>());
        }

        public ActionResult Results(string tagnames)
        {
            string[] tagarray = tagnames.Split(' ');
            
            List<Tag> tags = new List<Tag>();
            List<Photo> photos = new List<Photo>();

            tags.Where(x => tagarray.Contains(x.Name)).ToList();
            
            List<int> tagIds = tags.Select(x => x.Id).ToList();
            photos.Where(x => x.PublicProfile && x.PhotoTag.Any(y => tagIds.Contains(y.TagId))).ToList();

            var searchmodel = new SearchViewModel();

            searchmodel.Photos = photos.ToViewModel<List<PhotoViewModel>>();
            searchmodel.Tags = TagViewModel.ToViewModel(tags);

            return View(searchmodel);
        }

        public ActionResult NewFolderModal()
        {
            return View();
        }

        public ActionResult MultipleCardsModal()
        {
            return View();
        }

        public ActionResult PhotoEditModal()
        {
            return View();
        }
        public ActionResult MooOrderModal()
        {
            return View();
        }

        [Authorize]
        public ActionResult Account()
        {
            return View();
        }

        public ActionResult DeactivateModal()
        {
            return View();
        }

        public ActionResult PhotoDetails()
        {
            return View();
        }

        public ActionResult SocialAccounts()
        {
            return View();
        }

        [Authorize]
        public ActionResult BrandSettings()
        {
            return View();
        }

        public ActionResult UploadProfileImage()
        {
            return View();
        }

        [Authorize]
        public async Task UploadProfileImageAsync(IFormFile file, string photoName, string extension, [FromServices]UploadProfileImageCommand command)
        {
            var azureId = HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
            var photographerId = User.ID();

            var filePath = Path.GetTempFileName();

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    await command.ExecuteAsync(photographerId, stream, photoName, extension);
                }
            }
        }

    }
}
