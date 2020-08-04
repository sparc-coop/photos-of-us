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
    public class TagController : ControllerBase
    {
        [HttpGet]
        public async Task<List<Tag>> Get([FromServices] IDbRepository<Tag> tagRepository)
        {
            return await tagRepository.GetAllAsync();
        }
    }
}
