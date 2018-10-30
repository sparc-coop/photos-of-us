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
        private IRepository<Folder> _folder;

        public UserApiController(IRepository<User> userRepository, IRepository<Folder> folderRepository)
        {
            _user = userRepository;
            _folder = folderRepository;
        }
        

        [HttpGet]
        public UserViewModel Get()
        {
            //var user =  _user.Find(x => x.Id == User.ID()).ToViewModel<UserViewModel>();

            var azureId = User.AzureID();
            var user = _user.Include(x => x.UserIdentities).Find(x => x.UserIdentities.Any(y => y.AzureID == azureId)).ToViewModel<UserViewModel>();

            return user;
        }

        [HttpGet]
        [Route("GetOne/{id}")]
        public UserViewModel GetOne(int id)
        {
            return _user.Find(x => x.Id == id).ToViewModel<UserViewModel>();
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
            var folders = _folder.Where(x => x.PhotographerId == User.ID()).ToList();
            return folders.ToViewModel<List<FolderViewModel>>();
        }
    }
}
