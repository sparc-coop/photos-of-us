﻿using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;
using System.Security.Claims;
using Kuvio.Kernel.Architecture;
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
    
        
        [HttpPost]
        [Route("RenameFolder")]
        public FolderViewModel Put(int id, string newName)
        {
            var photographer = _user.Find(x => x.Id == User.ID());

            var folder = photographer.Folders.First(x => x.Id == id);
            folder.Name = newName;
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