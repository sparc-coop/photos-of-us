﻿using System;
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
        private IRepository<User> _users;
        private IRepository<Folder> _folders;

        public UserApiController(IRepository<User> userRepository, IRepository<Folder> folderRepository)
        {
            _users = userRepository;
            _folders = folderRepository;
        }
        

        [HttpGet]
        public UserViewModel Get()
        {
            var user = _users.Find(x => x.Id == User.ID());
            return user.ToViewModel<UserViewModel>();
        }

        [HttpPut]
        public IActionResult Put([FromBody]UserProfileUpdateCommandModel model, [FromServices]UserProfileUpdateCommand command)
        {
            var user = _users.Find(x => x.Id == User.ID());
            if (user.Id != model.Id)
                return Forbid();

            command.Execute(model);

            return Ok();
        }
        
        [HttpDelete]
        public void DeactivateAccount()
        {
            var user = _users.Find(x => x.Id == User.ID());
            user.Deactivate();
            _users.Commit();
        }

        [HttpPost]
        public void ReactivateAccount()
        {
            var user = _users.Find(x => x.Id == User.ID());
            user.Activate();
           _users.Commit();
        }

        [HttpGet]
        [Route("Folders")]
        public List<FolderViewModel> GetFolders(int id)
        {
            var user = _users.Find(x => x.Id == User.ID());
            return user.Folders.ToViewModel<List<FolderViewModel>>();
        }

        [HttpGet]
        [Route("GetProfilePhotos/{userId:int}")]
        public List<Photo> GetProfilePhotos(int userId)
        {
            return _users.Include(x => x.Photos)
                .Find(x => x.Id == userId)
                .Photos
                .Where(x => x.PublicProfile && !x.IsDeleted)
                .ToList();
        }

        [HttpPost]
        [Route("ViewedPricing/{userId:int}")]
        public void ViewedPricingInfo(int userId)
        {
            User user = _users.Find(x => x.Id == userId);
            user.PurchaseTour = true;
            _users.Commit();
        }

        [HttpPost]
        [Route("ViewedDashboard/{userId:int}")]
        public void ViewedDashboardTour(int userId)
        {
            User user = _users.Find(x => x.Id == userId);
            user.DashboardTour = true;
            _users.Commit();
        }

        [HttpPost]
        [Route("ViewedPhoto/{userId:int}")]
        public void ViewedPhotoTour(int userId)
        {
            User user = _users.Find(x => x.Id == userId);
            user.PhotoTour = true;
            _users.Commit();
        }
    }
}
