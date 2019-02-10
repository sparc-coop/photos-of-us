using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;
using System.Security.Claims;
using Kuvio.Kernel.Core;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Web.Utilities;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace PhotosOfUs.Web.Controllers.API
{
    [Authorize]
    [Route("api/Folder")]
    public class FolderApiController : Controller
    {
        private IRepository<User> _user;

        public FolderApiController(IRepository<User> userRepository)
        {
            _user = userRepository;
        }

        [HttpPost]
        [Route("{name}")]
        public FolderViewModel Post(string name)
        {
            var photographer = _user.Find(x => x.Id == User.ID());
            var folder = photographer.AddFolder(name);
            _user.Commit();

            return folder.ToViewModel<FolderViewModel>();
        }
    
        
        [HttpPut]
        public FolderViewModel Put(int id, string newName)
        {
            var photographer = _user.Find(x => x.Id == User.ID());

            var folder = photographer.Folders.First(x => x.Id == id);
            folder.Name = newName;
            _user.Commit();

            return folder.ToViewModel<FolderViewModel>();
        }

        [HttpDelete]
        public IActionResult DeleteFolder(int id)
        {
            var photographer = _user.Find(x => x.Id == User.ID());
            photographer.RemoveFolder(id);
            _user.Commit();

            return Ok();
        }
    }
}