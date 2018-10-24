using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Web.Models;
using PhotosOfUs.Web.Utilities;

namespace PhotosOfUs.Web.Controllers.API
{
    [Authorize]
    [Route("api/User")]
    public class UserApiController : Controller
    {
        private IRepository<User> _user;

        public UserApiController(IRepository<User> userRepository)
        {
            _user = userRepository;
        }
        

        [HttpGet]
        public UserViewModel Get()
        {
            return _user.Find(x => x.Id == User.ID()).ToViewModel<UserViewModel>();
        }

        [HttpPut]
        public IActionResult Put([FromBody]UserProfileUpdateCommandModel model, [FromServices]UserProfileUpdateCommand command)
        {
            if (User.ID() != model.Id)
                return Forbid();

            command.Execute(model);

            return Ok();
        }
        
        [HttpDelete]
        public UserViewModel DeactivateAccount()
        {
            User user = _user.Find(x => x.Id == User.ID());
            user.Deactivate();
            _user.Commit();

            return user.ToViewModel<UserViewModel>();
        }

        [HttpPost]
        public UserViewModel ReactivateAccount()
        {
            User user = _user.Find(x => x.Id == User.ID());
            user.Activate();
           _user.Commit();

            return user.ToViewModel<UserViewModel>();
        }

        public bool IsPhotoCodeAlreadyUsed(int photographerId, string code)
        {
            return _user.Find(x => x.Id == photographerId).Photo.Any(x => x.PhotographerId == photographerId && x.Code == code);
        }


        [HttpGet]
        [Route("GetFolders")]
        public List<FolderViewModel> GetFolders()
        {
            var folders = _user.Find(x => x.Id == User.ID()).Folder.ToList();
            return folders.ToViewModel<List<FolderViewModel>>();
        }
    }
}
