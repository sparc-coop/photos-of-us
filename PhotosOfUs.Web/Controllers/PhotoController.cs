using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;

namespace PhotosOfUs.Web.Controllers
{
    public class PhotoController : Controller
    {
        private PhotosOfUsContext _context;

        public PhotoController(PhotosOfUsContext context)
        {
            _context = context;
        }
        // GET: Photographer
        public ActionResult Purchase(int id)
        {
            return RedirectToAction("Dashboard");
        }
    }
}