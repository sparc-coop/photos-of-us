using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kuvio.Kernel.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotosOfUs.Core.Events;
using PhotosOfUs.Core.Photos;
using PhotosOfUs.Core.Photos.Queries;
using PhotosOfUs.Core.Users;
using PhotosOfUs.WA.Server.Utils;

namespace PhotosOfUs.WA.Server.Events
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    public class EventController : ControllerBase
    {
        //[HttpGet]
        //[Route("{id:int}")]
        //public async Task<Event> Get([FromServices] GetPhotoQuery getPhotoQuery, int id)
        //{
        //    return await getPhotoQuery.Execute(id);
        //}

        //[HttpGet]
        //[Route("{id:int}")]
        //public Photo Get([FromServices] IDbRepository<Photo> photoRepository, int id)
        //{
        //    return photoRepository.Include("Photographer", "PhotoTag.Tag").Where(x => x.Id == id).FirstOrDefault();
        //}

        [HttpGet]
        [Route("MyEvents")]
        public async Task<List<Event>> MyEvents([FromServices] IDbRepository<Event> eventRepository)
        {
            return await eventRepository.Query.Where(x => x.PhotographerId == 2).ToListAsync();
        }

        [Route("MyCode/{code}")]
        [AllowAnonymous]
        public async Task<List<Photo>> MyCode([FromServices] IDbRepository<Photo> photoRepository, string code)
        {
            // TODO: Write the actual method. This one is returning 5 random photos.
            return await photoRepository.Query.Take(5).ToListAsync();
        }
    }
}
