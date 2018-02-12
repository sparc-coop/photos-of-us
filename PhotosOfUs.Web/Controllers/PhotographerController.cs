using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;

namespace PhotosOfUs.Web.Controllers
{
    public class PhotographerController : Controller
    {
        private PhotosOfUsContext _context;

        public PhotographerController(PhotosOfUsContext context)
        {
            _context = context;
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

            var folder = new PhotoRepository(_context).GetPhotos(photographerId, id);

            return View(FolderViewModel.ToViewModel(folder));
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

        public ActionResult Upload()
        {
            return View();
        }

        public async Task UploadPhotoAsync(IFormFile file, string photoName, string photoCode)
        {
            Regex r = new Regex(@"^[A-Za-z0-9_-]+$", RegexOptions.IgnoreCase);
            var match  = r.Match(photoCode);

            if (//new PhotoRepository(_context).IsPhotoCodeAlreadyUsed(1, photoCode) || 
                string.IsNullOrEmpty(photoName) || string.IsNullOrEmpty(photoCode) ||
                match.Success == false)
                return;

            var filePath = Path.GetTempFileName();

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    await new PhotoRepository(_context).UploadFile(1, stream, file.FileName, photoCode);
                }
            }
        }

        public JsonResult VerifyIfCodeAlreadyUsed(string code)
        {
            //return Json( new { PhotoExisting = new PhotoRepository(_context).IsPhotoCodeAlreadyUsed(1, code) });
            return Json(new { PhotoExisting = false });
        }

        public ActionResult Test()
        {
            return View();
        }
    }
}
