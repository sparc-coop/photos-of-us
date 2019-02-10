using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kuvio.Kernel.Core;
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
        private readonly IRepository<User> _users;

        public UserApiController(IRepository<User> userRepository)
        {
            _users = userRepository;
        }
        

        [HttpGet]
        public UserViewModel Get()
        {
            var user = _users.Find(User.ID());
            return user.ToViewModel<UserViewModel>();
        }

        [HttpPut]
        public IActionResult Put([FromBody]UserProfileUpdateCommandModel model)
        {
            if (User.ID() != model.Id)
                return Forbid();

            _users.Execute(User.ID(), user =>
            {
                user.UpdateProfile(model.Email, model.FirstName, model.LastName, model.DisplayName, model.JobPosition, model.ProfilePhotoUrl, model.Bio);
                user.UpdateSocialMedia(model.Facebook, model.Instagram, model.Dribbble, model.Twitter);
            });

            return Ok();
        }
        
        [HttpDelete]
        public void DeactivateAccount()
        {
            _users.Execute(User.ID(), x => x.Deactivate());
        }

        [HttpPost]
        public void ReactivateAccount()
        {
            _users.Execute(User.ID(), x => x.Activate());
        }

        [HttpGet]
        [Route("Folders")]
        public List<FolderViewModel> GetFolders()
        {
            var user = _users.Find(User.ID());
            return user.Folders.ToViewModel<List<FolderViewModel>>();
        }

        [HttpGet]
        [Route("GetProfilePhotos/{userId:int}")]
        public List<Photo> GetProfilePhotos(int userId)
        {
            return _users.Find(userId)
                .Photos
                .Where(x => x.PublicProfile && !x.IsDeleted)
                .ToList();
        }

        [HttpPost]
        [Route("ViewedPricing/{userId:int}")]
        public void ViewedPricingInfo(int userId)
        {
            _users.Execute(userId, x => x.PurchaseTour = true);
        }

        [HttpPost]
        [Route("ViewedDashboard/{userId:int}")]
        public void ViewedDashboardTour(int userId)
        {
            _users.Execute(userId, x => x.DashboardTour = true);
        }

        [HttpPost]
        [Route("ViewedPhoto/{userId:int}")]
        public void ViewedPhotoTour(int userId)
        {
            _users.Execute(userId, x => x.PhotoTour = true);
        }
    }
}
