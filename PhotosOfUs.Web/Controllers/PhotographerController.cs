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
        [Authorize]
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

            var test = _context.Photo.Include(x=>x.PhotoTag).Where(x=>x.Id == 57).First();
            

            var tags2 = _context.PhotoTag.Include(x=>x.Tag).Where(x => x.PhotoId == 57).ToList();

            return View(model);
        }

        // GET: Photographer/Details/5
        [Authorize]
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

        [Authorize]
        public ActionResult Cards()
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;
            List<Card> pCards = _context.Card.Where(x => x.PhotographerId == photographerId).ToList();
            return View(pCards);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Export(List<int> ids)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;
            var cards = _context.Card
                .Include(x => x.Photographer)
                .Where(x => x.PhotographerId == photographerId && ids.Contains(x.Id)).ToList();

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

        [Authorize]
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



        [Authorize]
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
                    await new PhotoRepository(_context).UploadProfilePhotoAsync(photographerId, stream, photoName,string.Empty, extension);
                }
            }
        }

        public JsonResult VerifyIfCodeAlreadyUsed(string code)
        {
            return Json( new { PhotoExisting = new PhotoRepository(_context).IsPhotoCodeAlreadyUsed(1, code) });
        }

        [Authorize]
        public ActionResult Profile()
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;
            var photographer = _context.User.Find(photographerId);
            var photos = new PhotoRepository(_context).GetProfilePhotos(photographerId);
            
            return View(ProfileViewModel.ToViewModel(photos,photographer));
        }

        [Authorize]
        public ActionResult SalesHistory(string query = null)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (azureId == null) return View(SalesHistoryViewModel.ToViewModel(new List<Order>()));

            UserIdentity userIdentity = _context.UserIdentity.Find(azureId);

            // if the user can't be found make a safe but empty return
            if (userIdentity == null) return View(SalesHistoryViewModel.ToViewModel(new List<Order>()));


            //var photographerId = userIdentity.UserID;
            var photographerId = 1; //TODO: uncomment the above line and comment out this line when finished testing


            string queryString = HttpContext.Request.QueryString.ToString();
            SalesQueryModel sqm = new SalesQueryModel(queryString);

            var orders = new OrderRepository(_context).GetOrders(photographerId, sqm);
            SalesHistoryViewModel salesHistory = SalesHistoryViewModel.ToViewModel(orders);
            salesHistory.UserDisplayName = User.Identity.Name;
            Debug.WriteLine("Size of orders: {0}", salesHistory.Orders.Count);
            return View(salesHistory);
        }

        public ActionResult Search()
        {
            var photos = new PhotoRepository(_context).GetPublicPhotos();

            //var test = _context.Photo.Include(x => x.PhotoTag).Where(x => x.Id == 57).First();
            //var tags2 = _context.PhotoTag.Include(x => x.Tag).Where(x => x.PhotoId == 57).ToList();
            //var getalltags = new PhotoRepository(_context).GetAllTags();

            return View(PhotoViewModel.ToViewModel(photos));
        }

        public ActionResult Results(string tagnames)
        {
            string[] tagarray = tagnames.Split(' ');

            var photos = new PhotoRepository(_context).GetPublicPhotosByTag(tagarray);



            //var test = _context.Photo.Include(x => x.PhotoTag).Where(x => x.Id == 57).First();
            //var tags2 = _context.PhotoTag.Include(x => x.Tag).Where(x => x.PhotoId == 57).ToList();
            //var getalltags = new PhotoRepository(_context).GetAllTags();

            return View(PhotoViewModel.ToViewModel(photos)); ;
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

        public ActionResult PhotoDetails()
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

        [Authorize]
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
