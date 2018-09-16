using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Repositories;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/AccountSettings")]
    public class AccountSettingsApiController : Controller
    {
        private PhotosOfUsContext _context;
        private UserRepository _userRepository;

        public AccountSettingsApiController(PhotosOfUsContext context, UserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("ProfileSettings")]
        public ProfileSettingsViewModel GetProfileSettings(int photoId)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _context.UserIdentity.Find(azureId).UserID;

            var user = _userRepository.Find(userId);

            return ProfileSettingsViewModel.ToViewModel(user);
        }

        [HttpPost]
        [Route("ProfileSettings")]
        public HttpResponseMessage SaveProfileSettings(ProfileSettingsViewModel model)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _context.UserIdentity.Find(azureId).UserID;

            if(userId != model.UserId)
                return new HttpResponseMessage(HttpStatusCode.Forbidden);

            var success = _userRepository.UpdateAccountProfileSettings(model);

            return !success ? new HttpResponseMessage(HttpStatusCode.Conflict) : new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
