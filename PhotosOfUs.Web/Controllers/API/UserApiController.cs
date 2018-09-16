using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/User")]
    public class UserApiController : Controller
    {
        private PhotosOfUsContext _context;

        public UserApiController(PhotosOfUsContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetUser")]
        public UserViewModel GetUser()
        {
            string azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserIdentity userIdentity = new UserRepository(_context).GetUser(azureId);
            User user = new UserRepository(_context).Find(userIdentity.UserID);

            return UserViewModel.ToViewModel(user);           
        }

        [HttpPost]
        [Route("Deactivate/{userId:int}")]
        public UserViewModel DeactivateAccount(int userId)
        {
            User user = new UserRepository(_context).Find(userId);
            user.IsDeactivated = true;

            _context.Update(user);
            _context.SaveChanges();

            return UserViewModel.ToViewModel(user);
        }

        [HttpPost]
        [Route("Reactivate/{userId:int}")]
        public UserViewModel ReactivateAccount(int userId)
        {
            User user = new UserRepository(_context).Find(userId);
            user.IsDeactivated = false;

            _context.Update(user);
            _context.SaveChanges();

            return UserViewModel.ToViewModel(user);
        }
    }
}
