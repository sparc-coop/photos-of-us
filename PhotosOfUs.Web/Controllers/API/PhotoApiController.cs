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

        [HttpGet]
        [Route("GetPhotographer/{id:int}")]
        public UserViewModel GetPhotographer(int id)
        {
            var photographer = new UserRepository(_context).Find(id);
            return UserViewModel.ToViewModel(photographer);
        }

        [HttpGet]
        [Route("GetFolders")]
        public List<FolderViewModel> GetFolders()
        {
            //todo: updata photographerId

            var photographerId = 1;
            var folders = new PhotoRepository(_context).GetFolders(photographerId);

            return FolderViewModel.ToViewModel(folders);
        }
    }
}
