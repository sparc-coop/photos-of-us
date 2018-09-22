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
            var photographer = _context.UserIdentity.Find(azureId);

            photographer.AddFolder(name);
            _context.SaveChanges();

            return FolderViewModel.ToViewModel(folder);
        }

        // TODO: This should be a Put        
        [Route("RenameFolder")]
        [HttpPost]
        public FolderViewModel Post([FromBody]FolderRenameViewModel model)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographer = _context.UserIdentity.Find(azureId);

            var folder = photographer.Folder.SingleOrDefault(x=> x.Id == model.Id);
            folder.Name = model.NewName;
            _context.SaveChanges();

            return FolderViewModel.ToViewModel(folder);
        }

        [HttpPost]
        [Route("DeleteFolder/{id:int}")]
        public IActionResult DeleteFolder(int id)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographer = _context.UserIdentity.Find(azureId);
            
            photographer.RemoveFolder(id);
            _context.SaveChanges();

            return Ok();
        }
    }
}