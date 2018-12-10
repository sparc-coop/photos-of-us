using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Web.Utilities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System;
using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Model.Photos.Commands;


namespace PhotosOfUs.Web.Controllers
{
    [Authorize]
    [Route("/api/Photographer")]
    public class PhotographerController : Controller
    {
        [HttpPost]
        [Route("ProfilePhoto")]
        public async Task<UploadPhotoCommand.UploadPhotoCommandResult> UploadProfilePhotoAsync(IFormFile file, [FromServices]UploadPhotoCommand command)
        {
            return await command.ExecuteAsync(User.ID(), file.FileName, file.OpenReadStream());
        }

        [Authorize]
        public async Task UploadProfileImageAsync(IFormFile file, string photoName, string extension, [FromServices]UploadProfileImageCommand command)
        {
            var azureId = HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
            var photographerId = User.ID();

            var filePath = Path.GetTempFileName();

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    await command.ExecuteAsync(photographerId, stream, photoName, extension);
                }
            }
        }

    }
}
