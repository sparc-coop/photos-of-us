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


namespace PhotosOfUs.Web.Controllers
{
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

        public ActionResult Dashboard()
        {
            var photographerId = 1;

            var folders = new PhotoRepository(_context).GetFolders(photographerId);

            return View(FolderViewModel.ToViewModel(folders));
        }

        // GET: Photographer/Details/5
        public ActionResult Photos(int id)
        {
            var photographerId = 1;
            var folderId = 1;

            var folder = new PhotoRepository(_context).GetPhotos(photographerId, folderId);

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
            var photographerId = 1;
            List<Card> pCards = _context.Card.Where(x => x.PhotographerId == photographerId).ToList();
            return View(pCards);
        }

        public ActionResult ExportNewCard()
        {
            var photographerId = 1;
            var cardR = new CardRepository(_context);
            CardViewModel newCard = CardViewModel.ToViewModel(cardR.Add(photographerId));
            
            var model = new CardViewModel { Code = newCard.Code, Url = newCard.Url};
            return new ActionAsPdf("CardToExport", model) {
                FileName = "PoU-Card-" + newCard.Code + ".pdf",
                PageSize = Size.Letter,
                PageOrientation = Orientation.Landscape,
                PageMargins = { Left = 0, Right = 0 },
                //ContentDisposition = ContentDisposition.Inline
            };
        }

        public ActionResult CardToExport(CardViewModel model)
        {
            return View(model);
        }

        public ActionResult Upload()
        {
            return View();
        }

        //public async Task UploadPhotoAsync(IFormFile file, string photoName, string photoCode, string extension)
        //{
        //    Regex r = new Regex(@"^[A-Za-z0-9_-]+$", RegexOptions.IgnoreCase);
        //    var match = r.Match(photoCode);

        //    if (new PhotoRepository(_context).IsPhotoCodeAlreadyUsed(1, photoCode) || 
        //        string.IsNullOrEmpty(photoName) || string.IsNullOrEmpty(photoCode) ||
        //        match.Success == false)
        //        return;

        //    var filePath = Path.GetTempFileName();

        //    if (file.Length > 0)
        //    {
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //            await new PhotoRepository(_context).UploadFile(1, stream, photoName, photoCode, extension);
        //        }
        //    }
        //}
        public async Task UploadPhotoAsync(IFormFile file, string photoName, string photoCode, string extension)
        {
            var ocr = new OCR(_hostingEnvironment);
            ocr.GetOCRResult(file);
        }

        public JsonResult VerifyIfCodeAlreadyUsed(string code)
        {
            return Json( new { PhotoExisting = new PhotoRepository(_context).IsPhotoCodeAlreadyUsed(1, code) });
        }
    }
}
