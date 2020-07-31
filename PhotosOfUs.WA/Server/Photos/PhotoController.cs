using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kuvio.Kernel.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotosOfUs.Core.Photos;

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
        public async Task<List<Photo>> GetPhotos([FromServices] IDbRepository<Photo> _photoRepository,  int count)
        {
            Random rand = new Random();
            int toSkip = rand.Next(0, (await _photoRepository.CountAsync(x => x.Id > 0)) - count);

            var photos = _photoRepository.Query.Skip(toSkip).Take(count).ToList();
            return photos;
        }
    }
}
