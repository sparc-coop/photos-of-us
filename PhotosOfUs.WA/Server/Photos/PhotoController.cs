using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kuvio.Kernel.Core;
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
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return Summaries;
        }

        [Route("Dashboard/{count:int}")]
        public async Task<List<RandomPhotoModel>> GetPhotos([FromServices] GetRandomPhotosQuery getRandomPhotosQuery,  int count)
        {
            return await getRandomPhotosQuery.Execute(30);
        }
    }
}
