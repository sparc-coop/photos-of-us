using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;
using System.Security.Claims;
using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Web.Utilities;
using System.Collections.Generic;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Folder")]
    public class FolderApiController : Controller
    {
        private PhotosOfUsContext _context;
        private IRepository<User> _user;
        private readonly IRepository<Folder> _folder;

        public FolderApiController(PhotosOfUsContext context, IRepository<User> userRepository, IRepository<Folder> folderRepository)
        {
            _context = context;
            _user = userRepository;
            _folder = folderRepository;
        }

        [HttpPost]
        [Route("{name}/{userId:int}")]
        public FolderViewModel Post(string name, int userId)
        {
            var newuserId = userId;
            var photographer = _user.Find(x => x.Id == userId);
            var newPhotographer = photographer;
            var folder = photographer.AddFolder(name);
            _user.Commit();

            return folder.ToViewModel<FolderViewModel>();
        }
    
        
        [HttpPost]
        [Route("RenameFolder")]
        public FolderViewModel Put([FromBody]FolderRenameViewModel model)
        {
            var photographer = _user.Find(x => x.Id == model.UserId);

            var folder = _folder.Find(x => x.Id == model.Id);
            folder.Name = model.NewName;
            _user.Commit();

            return folder.ToViewModel<FolderViewModel>();
        }

        [HttpPost]
        [Route("DeleteFolder/{id:int}/{userId:int}")]
        public IActionResult DeleteFolder(int id, int userId)
        {
            var photographer = _user.Find(x => x.Id == userId);

            photographer.RemoveFolder(id);
            _user.Commit();

            return Ok();
        }
    }
}