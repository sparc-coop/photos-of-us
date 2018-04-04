using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Folder")]
    public class FolderApiController : Controller
    {
        private PhotosOfUsContext _context;

        public FolderApiController(PhotosOfUsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public FolderViewModel Post(string name)
        {
            //todo: photographerId
            var folder = new FolderRepository(_context).Add(name, 1);

            return FolderViewModel.ToViewModel(folder);
        }

        [HttpPost]
        [Route("DeleteFolder/{id:int}")]
        public IActionResult DeleteFolder(int id)
        {
            new FolderRepository(_context).Delete(id);

            return Ok();
        }
    }
}