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
        private readonly FolderRepository _folderRepository;

        public FolderApiController(PhotosOfUsContext context, FolderRepository folderRepository)
        {
            _context = context;
            _folderRepository = folderRepository;
        }

        [HttpPost]
        public FolderViewModel Post(string name)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;

            var folder = _folderRepository.Add(name,photographerId);

            return FolderViewModel.ToViewModel(folder);
        }

        [Route("RenameFolder")]
        [HttpPost]
        public FolderViewModel Post([FromBody]FolderRenameViewModel model)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;

            var folder = _folderRepository.Rename(model.Id,model.NewName, photographerId);

            return FolderViewModel.ToViewModel(folder);
        }

        [HttpPost]
        [Route("DeleteFolder/{id:int}")]
        public IActionResult DeleteFolder(int id)
        {
            _folderRepository.Delete(id);

            return Ok();
        }
    }
}