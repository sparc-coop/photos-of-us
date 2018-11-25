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
            var user = _user.Include(x => x.UserIdentities).Find(x => x.UserIdentities.Any(y => y.AzureID == User.AzureID())).ToViewModel<UserViewModel>();

            return user;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetOne/{id}")]
        public UserViewModel GetOne(int id)
        {
            return _user.Find(x => x.Id == id).ToViewModel<UserViewModel>();
        }

        [HttpPut]
        public IActionResult Put([FromBody]UserProfileUpdateCommandModel model, [FromServices]UserProfileUpdateCommand command)
        {
            var user = _user.Include(x => x.UserIdentities).Find(x => x.UserIdentities.Any(y => y.AzureID == User.AzureID()));
            if (user.Id != model.Id)
                return Forbid();

            command.Execute(model);

            return Ok();
        }
        
        [HttpDelete]
        public UserViewModel DeactivateAccount()
        {
            //User user = _user.Find(x => x.Id == User.ID());
            var user = _user.Include(x => x.UserIdentities).Find(x => x.UserIdentities.Any(y => y.AzureID == User.AzureID()));
            user.Deactivate();
            _user.Commit();

            return user.ToViewModel<UserViewModel>();
        }

        [HttpPost]
        public UserViewModel ReactivateAccount()
        {
            //User user = _user.Find(x => x.Id == User.ID());
            var user = _user.Include(x => x.UserIdentities).Find(x => x.UserIdentities.Any(y => y.AzureID == User.AzureID()));
            user.Activate();
           _user.Commit();

            return user.ToViewModel<UserViewModel>();
        }

        [HttpGet]
        [Route("GetFolders/{id:int}")]
        public List<FolderViewModel> GetFolders(int id)
        {
            //var folders = _folder.Where(x => x.PhotographerId == User.ID()).ToList();
            var folders = _folder.Where(x => x.PhotographerId == id).ToList();
            return folders.ToViewModel<List<FolderViewModel>>();
        }

        [HttpPost]
        [Route("ViewedPricing/{userId:int}")]
        public void ViewedPricingInfo(int userId)
        {
            User user = _user.Find(x => x.Id == userId);
            user.PurchaseTour = true;
            _user.Commit();
        }

        [HttpPost]
        [Route("ViewedDashboard/{userId:int}")]
        public void ViewedDashboardTour(int userId)
        {
            User user = _user.Find(x => x.Id == userId);
            user.DashboardTour = true;
            _user.Commit();
        }

        [HttpPost]
        [Route("ViewedPhoto/{userId:int}")]
        public void ViewedPhotoTour(int userId)
        {
            User user = _user.Find(x => x.Id == userId);
            user.PhotoTour = true;
            _user.Commit();
        }
    }
}
