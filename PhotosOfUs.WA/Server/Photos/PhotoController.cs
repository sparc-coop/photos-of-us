using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kuvio.Kernel.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotosOfUs.Core.Photos;
using PhotosOfUs.Core.Photos.Queries;

namespace PhotosOfUs.WA.Server.Photos
{
    [ApiController]
    [Route("[controller]")]
    public class PhotoController : ControllerBase
    {
        [HttpGet]
        [Route("{id:int}")]
        public async Task<PhotoModel> Get([FromServices] GetPhotoQuery getPhotoQuery, int id)
        {
            return await getPhotoQuery.Execute(id);
        }

        //[HttpGet]
        //[Route("{id:int}")]
        //public Photo Get([FromServices] IDbRepository<Photo> photoRepository, int id)
        //{
        //    return photoRepository.Include("Photographer", "PhotoTag.Tag").Where(x => x.Id == id).FirstOrDefault();
        //}

        [Route("Dashboard/{count:int}")]
        [AllowAnonymous]
        public async Task<List<RandomPhotoModel>> GetPhotos([FromServices] GetRandomPhotosQuery getRandomPhotosQuery, int count)
        {
            return await getRandomPhotosQuery.Execute(count);
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
