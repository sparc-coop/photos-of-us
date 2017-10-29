using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Repositories;

namespace PhotosOfUs.Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/PhotographerApi")]
    public class PhotographerApiController : Controller
    {
        // GET: api/PhotographerApi
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/PhotographerApi/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }


        [HttpPost]
        [Route("{photographerId:int}/Uploads")]
        public async Task<string> Upload(int photographerId)
        {
            //var stream = await GetUploadStream();
            //var model = await new PhotoRepository().Upload(photographerId, stream.Stream, stream.Filename);

            //return new FileViewModel(model);
            return null;
        }
    }
}
