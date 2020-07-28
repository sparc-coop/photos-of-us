using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        //public List<Photo> GetPhotos(int count)
        //{
        //    var random = new Random();
        //    var photos = EventRepository.Query.SelectMany(x => x.Photos.Where(y => !y.IsDeleted));
        //    var photoCount = photos.Count();
        //    var photoList = new List<Photo>();

        //    for (var i = 0; i < count; i++)
        //    {
        //        var toSkip = random.Next(0, photoCount);
        //        var photo = photos.Skip(toSkip).Take(1).FirstOrDefault();
        //        if (photo != null)
        //        {
        //            photoList.Add(photo);
        //        }
        //    }

        //    return photoList;
        //}
    }
}
