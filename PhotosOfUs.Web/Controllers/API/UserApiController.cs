using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;
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
    [Route("api/User")]
    public class UserApiController : Controller
    {
        private IRepository<User> _userRepository;

        public UserApiController(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Get()
        {
            var user = _userRepository.Find(User.ID()).ToViewModel<UserProfileUpdateCommandModel>();
            return Ok(user);
        }

        [HttpPut]
        [Route("")]
        public IActionResult Put([FromBody]UserProfileUpdateCommandModel model, [FromServices]UserProfileUpdateCommand command)
        {
            if (User.ID() != model.Id)
                return Forbid();

            command.Execute(model);

            return Ok();
        }
        
        [HttpGet]
        [Route("GetUser")]
        public UserViewModel GetUser()
        {
            string azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserIdentity userIdentity = _userRepository.GetUser(azureId);
            User user = _userRepository.Find(userIdentity.UserID);

            return UserViewModel.ToViewModel(user);           
        }

        [HttpPost]
        [Route("Deactivate/{userId:int}")]
        public UserViewModel DeactivateAccount(int userId)
        {
            User user = _userRepository.Find(userId);
            user.IsDeactivated = true;

            _context.Update(user);
            _context.SaveChanges();

            return UserViewModel.ToViewModel(user);
        }

        [HttpPost]
        [Route("Reactivate/{userId:int}")]
        public UserViewModel ReactivateAccount(int userId)
        {
            User user = _userRepository.Find(userId);
            user.IsDeactivated = false;

            _context.Update(user);
            _context.SaveChanges();

            return UserViewModel.ToViewModel(user);
        }
    }
}
