using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Photo")]
    public class PhotoApiController : Controller
    {
        private PhotosOfUsContext _context;

        public PhotoApiController(PhotosOfUsContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("{photoId:int}")]
        public PhotoViewModel GetPhoto(int photoId)
        {
            var photo = new PhotoRepository(_context).GetPhoto(photoId);
            return PhotoViewModel.ToViewModel(photo);
        }

        [HttpGet]
        [Route("GetPrintTypes")]
        public List<PrintTypeViewModel> GetPrintTypes()
        {
            var printType = new PhotoRepository(_context).GetPrintTypes();
            return PrintTypeViewModel.ToViewModel(printType);
        }
    }
}
