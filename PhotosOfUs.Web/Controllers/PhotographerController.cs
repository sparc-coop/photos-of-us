using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.Services;
using PhotosOfUs.Model.ViewModels;
using Rotativa.NetCore;
using Rotativa.NetCore.Options;


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
    }
}