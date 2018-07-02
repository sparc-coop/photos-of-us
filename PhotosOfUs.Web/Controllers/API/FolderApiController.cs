using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;

            var folder = new FolderRepository(_context).Add(name,photographerId);

            return FolderViewModel.ToViewModel(folder);
        }

        [Route("RenameFolder")]
        [HttpPost]
        public FolderViewModel Post([FromBody]FolderRenameViewModel model)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;

            var folder = new FolderRepository(_context).Rename(model.Id,model.NewName, photographerId);

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