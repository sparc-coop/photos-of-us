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
        private readonly IRepository<User> _users;

        public FolderApiController(IRepository<User> userRepository)
        {
            _users = userRepository;
        }

        [HttpPost]
        [Route("{name}")]
        public void Post(string name)
        {
            _users.Execute(User.ID(), x => x.AddFolder(name));
        }
    
        
        [HttpPut]
        public void Put(int id, string newName)
        {
            _users.Execute(User.ID(), user =>
            {
                var folder = user.Folders.First(x => x.Id == id);
                folder.Name = newName;
            });
        }

        [HttpDelete]
        public IActionResult DeleteFolder(int id)
        {
            _users.Execute(User.ID(), x => x.RemoveFolder(id));

            return Ok();
        }
    }
}