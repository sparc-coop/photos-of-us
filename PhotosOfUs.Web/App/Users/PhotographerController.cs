﻿using System.Collections.Generic;
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
    [Area("Users")]
    public class PhotographerController : Controller
    {
        private PhotosOfUsContext _context;
        private readonly IRepository<Photo> _photo;
        private readonly IRepository<User> _user;
        private readonly IRepository<Card> _card;
        private readonly IRepository<Folder> _folder;

        public PhotographerController(PhotosOfUsContext context, IRepository<Photo> photoRepository, 
        IRepository<User> userRepository, IRepository<Card> cardRepository, IRepository<Folder> folderRepository)
        {
            _context = context;
            _photo = photoRepository;
            _user = userRepository;
            _card = cardRepository;
            _folder = folderRepository;
        }

        [Authorize]
        public ActionResult Photos(int id)
        {
            var photographer = _user.Find(x => x.Id == User.ID());
            Folder folder = _folder.Include(x => x.Photo).Find(x => x.PhotographerId == photographer.Id && x.Id == id);

            return View(folder.ToViewModel<FolderViewModel>());
        }
        
        [Authorize]
        public ActionResult Cards()
        {
            var azureId = User.AzureID();
            var user = _user.Include(x => x.UserIdentities).Find(x => x.UserIdentities.Any(y => y.AzureID == azureId));
            List<Card> pCards = _user.Find(x => x.Id == user.Id).Card.Where(y => y.PhotographerId == user.Id).ToList();
            return View(pCards);
        }

        [HttpPost]
        [Authorize]
        [Produces("application/json")]
        public ActionResult Export(List<int> ids)
        {
            //var cards = _user.Find(x => x.Id == User.ID()).Card
            //    .Where(x => x.PhotographerId == User.ID() && ids.Contains(x.Id)).ToList();
            var user = _user.Include(x => x.UserIdentities).Find(x => x.UserIdentities.Any(y => y.AzureID == User.AzureID()));
            var cards = _card.Include(x => x.Photographer).Where(x => x.PhotographerId == user.Id && ids.Contains(x.Id)).ToList();

            var json = JsonConvert.SerializeObject(cards.Select(CardViewModel.ToViewModel).ToList());
            return new ActionAsPdf("ExportPdf", new { json })
            {
                FileName = "Cards.pdf",
                PageSize = Size.Letter,
                PageOrientation = Orientation.Landscape,
                PageMargins = { Left = 0, Right = 0 },
                Cookies = Request.Cookies.ToDictionary(x => x.Key, x => x.Value)
            };

        }

        public ActionResult ExportPdf(string json)
        {
            var newString = json;
            var cards = JsonConvert.DeserializeObject<List<CardViewModel>>(json);
            return View(cards);
        }

        public ActionResult Upload()
        {
            return View();
        }

        public ActionResult UploadProfilePhoto()
        {
            return View();
        }

        public ActionResult BulkEditModal()
        {
            return View();
        }

        public ActionResult PriceModal()
        {
            return View();
        }

        [Authorize]
        public async Task<AzureCognitiveViewModel> UploadPhotoAsync(IFormFile file, string photoName, string photoCode, string extension, int folderId, int price, string tags,
        [FromServices]UploadPhotoCommand command)
        {
            RootObject tagsfromazure = null;

            var ac = new AzureCognitive();
            byte[] imgbytes = AzureCognitive.TransformImageIntoBytes(file);
            tagsfromazure = await ac.MakeRequest(imgbytes, "tags");

            if (string.IsNullOrEmpty(photoCode))
            {
                var codefromazure = await ac.MakeRequest(imgbytes, "ocr");

                var suggestedtags = ac.ExtractTags(tagsfromazure);
                var code = ac.ExtractCardCode(codefromazure);

                return AzureCognitiveViewModel.ToViewModel(code, suggestedtags);
            }
            
            var listoftags = new List<TagViewModel>();
            if (tags != null)
            {
                List<string> result = tags.Split(' ').ToList();

                foreach (string obj in result)
                {
                    listoftags.Add(new TagViewModel() { Name = obj, text = obj });
                }
            }
            
            var filePath = Path.GetTempFileName();

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    await command.ExecuteAsync(User.ID(), stream, photoName, photoCode, extension, folderId, price, tagsfromazure, TagViewModel.ToEntity(listoftags));
                }
            }

            return new AzureCognitiveViewModel();
        }



        [Authorize]
        public async Task<AzureCognitiveViewModel> UploadProfilePhotoAsync(IFormFile file, string photoName, string price, string extension, string tags, [FromServices]UploadPhotoCommand command)
        {
            //User photographer = _user.Find(x => x.Id == User.ID());
            User photographer = _user.Include(x => x.UserIdentities).Find(x => x.UserIdentities.Any(y => y.AzureID == User.AzureID()));
            RootObject tagsfromazure = null;
            List<string> suggestedtags = null;

            var ac = new AzureCognitive(_context, _card);
            var imgbytes = AzureCognitive.TransformImageIntoBytes(file);
            //tagsfromazure = await ac.MakeRequest(imgbytes, "tags");
            tagsfromazure = await ac.MakeRequest(imgbytes, "tags");

            if(tagsfromazure.tags != null)
            {
                suggestedtags = ac.ExtractTags(tagsfromazure);

                if (string.IsNullOrEmpty(tags))
                {
                    return AzureCognitiveViewModel.ToViewModel(suggestedtags);
                }
            }

            double addPrice = double.Parse(price);

            var listoftags = new List<TagViewModel>();
            List<string> result = tags.Split(' ').ToList();

            foreach (string obj in result)
            {
                listoftags.Add(new TagViewModel() { Name = obj, text = obj });
            }

            var filePath = Path.GetTempFileName();

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    int folderId = photographer.PublicFolder.Id;
                    await file.CopyToAsync(stream);
                    if (photographer.PublicFolder == null)
                    {
                        folderId = photographer.AddFolder("Public").Id;
                        _photo.Commit();
                    }
                    //await _user.UploadProfilePhotoAsync(photographer.Id, stream, photoName, string.Empty, addPrice, photographer.PublicFolder, extension, tagsfromazure, listoftags);
                    await command.ExecuteAsync(User.ID(), stream, photoName, string.Empty, extension, folderId, addPrice, tagsfromazure, TagViewModel.ToEntity(listoftags));
                    }
            }

            return AzureCognitiveViewModel.ToViewModel(suggestedtags);
        }

        public JsonResult VerifyIfCodeAlreadyUsed(string code)
        {
            var azureId = HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
            int id = _user.Find(x => x.AzureId == azureId).Id;
            return Json(new { PhotoExisting = _photo.Find(x => x.PhotographerId == id && x.Code == code)});
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