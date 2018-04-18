﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.Services;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Web.Utilities;
using Rotativa.NetCore;
using Rotativa.NetCore.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Newtonsoft.Json;

namespace PhotosOfUs.Web.Controllers
{
   [Authorize]
    public class PhotographerController : Controller
    {
        private PhotosOfUsContext _context;
        private IHostingEnvironment _hostingEnvironment;

        public PhotographerController(PhotosOfUsContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        
        // GET: Photographer
        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;

            PhotographerDashboardViewModel model = new PhotographerDashboardViewModel();
            model.PhotographerId = photographerId;
            model.Name = User.Identity.Name;

            return View(model);
        }

        // GET: Photographer/Details/5
        public ActionResult Photos(int id)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;

            var folder = new PhotoRepository(_context).GetPhotos(photographerId, id);

            return View(FolderViewModel.ToViewModel(folder));
        }

        public ActionResult Photo(int id)
        {
            var photo = new PhotoRepository(_context).GetPhoto(id);
            return View(PhotoViewModel.ToViewModel(photo));
        }

        public ActionResult Code(int id)
        {
            return View();
        }

        public ActionResult Code2(int id)
        {
            return View();
        }

        public ActionResult Code3(int id)
        {
            return View();
        }

        public ActionResult Code4(int id)
        {
            return View();
        }

        public ActionResult PhotoCode(string code)
        {
            var photos = new PhotoRepository(_context).GetPhotosByCode(code);
            return View(PhotoViewModel.ToViewModel(photos));
        }

        // GET: Photographer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Photographer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Photographer/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Photographer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Photographer/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Photographer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Cards()
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;
            List<Card> pCards = _context.Card.Where(x => x.PhotographerId == photographerId).ToList();
            return View(pCards);
        }

        public ActionResult ExportNewCard()
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographer = _context.UserIdentity.Find(azureId);
            var nCard = new CardRepository(_context).Add(photographer.UserID);
            _context.Entry(nCard).Reference(c => c.Photographer).Load();

            CardViewModel newCard = CardViewModel.ToViewModel(nCard);
            List<CardViewModel> lCards = new List<CardViewModel>();
            lCards.Add(newCard);
            var json = JsonConvert.SerializeObject(lCards);

            return new ActionAsPdf("CardToExport", new { json }) {
                FileName = "PoU-Card-" + newCard.Code + ".pdf",
                PageSize = Size.Letter,
                PageOrientation = Orientation.Landscape,
                PageMargins = { Left = 0, Right = 0 },
                //ContentDisposition = ContentDisposition.Inline
            };
        }

        public ActionResult ExportExistingCard(int id)
        {
            Card eCard = _context.Card.Find(id);
            _context.Entry(eCard).Reference(c => c.Photographer).Load();
            CardViewModel model = CardViewModel.ToViewModel(eCard);

            List<CardViewModel> lCards = new List<CardViewModel>();
            lCards.Add(model);
            var json = JsonConvert.SerializeObject(lCards);

            return new ActionAsPdf("CardToExport", new { json })
            {
                FileName = "PoU-Card-" + model.Code + ".pdf",
                PageSize = Size.Letter,
                PageOrientation = Orientation.Landscape,
                PageMargins = { Left = 0, Right = 0 },
                //ContentDisposition = ContentDisposition.Inline
            };
        }

        public ActionResult ExportMultipleCards(int quantity)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographer = _context.UserIdentity.Find(azureId);
            var nCard = new CardRepository(_context).AddMultiple(photographer.UserID,quantity);
            
            List<CardViewModel> newCards = nCard.Select(x=>CardViewModel.ToViewModel(x)).ToList();
            var json = JsonConvert.SerializeObject(newCards);

            return new ActionAsPdf("CardToExport", new { json })
            {
                FileName = "PoU-Cards-" + DateTime.Now.ToString("HHmmss") + ".pdf",
                PageSize = Size.Letter,
                PageOrientation = Orientation.Landscape,
                PageMargins = { Left = 0, Right = 0 },
                //ContentDisposition = ContentDisposition.Inline
            };
        }

        public ActionResult CardToExport(string json)
        {
            List<CardViewModel> model = JsonConvert.DeserializeObject<List<CardViewModel>>(json);
            return View(model);
        }

        public ActionResult Upload()
        {
            return View();
        }

        public ActionResult UploadProfilePhoto()
        {
            return View();
        }

        public async Task<string> UploadPhotoAsync(IFormFile file, string photoName, string photoCode, string extension, int folderId)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;

            if (string.IsNullOrEmpty(photoCode))
            {
                var ocr = new OCR(_context,_hostingEnvironment);
                var ocrResult = ocr.GetOCRResult(file,photographerId);
                return ocrResult.Code;
            }
            
            var filePath = Path.GetTempFileName();

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    await new PhotoRepository(_context).UploadFile(photographerId, stream, photoName, photoCode, extension, folderId);
                }
            }

            return "";
        }




        public async Task UploadProfilePhotoAsync(IFormFile file, string photoName, string extension)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;
            
            var filePath = Path.GetTempFileName();

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    await new PhotoRepository(_context).UploadProfilePhotoAsync(photographerId, stream, photoName,string.Empty, extension, true);
                }
            }
        }

        public JsonResult VerifyIfCodeAlreadyUsed(string code)
        {
            return Json( new { PhotoExisting = new PhotoRepository(_context).IsPhotoCodeAlreadyUsed(1, code) });
        }

        public ActionResult Profile()
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;
            var photographer = _context.User.Find(photographerId);
            var photos = new PhotoRepository(_context).GetProfilePhotos(photographerId);
            
            return View(ProfileViewModel.ToViewModel(photos,photographer));
        }

        public ActionResult SalesHistory()
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _context.UserIdentity.Find(azureId).UserID;
            var user = _context.User.Find(userId);

            var orders = new OrderRepository(_context).GetOrders(user.Id);

            return View(SalesHistoryViewModel.ToViewModel(user, orders));
        }

        public ActionResult NewFolderModal()
        {
            return View();
        }

        public ActionResult MultipleCardsModal()
        {
            return View();
        }

        public ActionResult Account()
        {
            return View();
        }

        public ActionResult SocialAccounts()
        {
            return View();
        }

        public ActionResult BrandSettings()
        {
            return View();
        }

        public ActionResult UploadProfileImage()
        {
            return View();
        }

        public async Task UploadProfileImageAsync(IFormFile file, string photoName, string extension)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;

            var filePath = Path.GetTempFileName();

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    await new UserRepository(_context).UpdateProfileImageAsync(photographerId, stream, photoName, string.Empty, extension);
                }
            }
        }

    }
}
