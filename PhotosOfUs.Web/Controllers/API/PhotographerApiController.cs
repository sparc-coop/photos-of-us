using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Web.Utilities;
using Kuvio.Kernel.Auth;
using Kuvio.Kernel.Architecture;

namespace PhotosOfUs.Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Photographer")]
    public class PhotographerApiController : Controller
    {
        private readonly PhotoRepository _photoRepository;

        public PhotographerApiController(PhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }
       
        [HttpPost]
        [Route("GetTagsByPhotos")]
        public List<TagViewModel> GetTagsByPhotos([FromBody]List<int> photos)
        {
            var repo = _photoRepository;

            var tags = repo.GetTagsByPhotos(photos);

            var tagsmodel = TagViewModel.ToViewModel(tags);

            return tagsmodel;
        }

        [HttpGet]
        [Route("GetPhotoPrice/{photoId:int}")]
        public decimal? GetPhotoPrice(int photoId)
        {
            var photo = _photoRepository.GetPhoto(photoId);

            return photo.Price;
        }

        [HttpPost]
        [Route("SavePhotoPrice/{photoId:int}/{newPrice:decimal}")]
        public void GetPhotoPrice(int photoId, decimal newPrice)
        {
            _photoRepository.UpdatePrice(photoId, newPrice);
        }

        [HttpPost]
        [Route("AddTags")]
        public void AddTags([FromBody]List<TagViewModel> tags)
        {
            _photoRepository.AddTags(tags);
        }

        [HttpPost]
        [Route("EditPhotos")]
        public void EditPhotos([FromBody]PhotoTagViewModel photosviewmodel)
        {
            _photoRepository.EditTags(photosviewmodel);
        }

        [Route("DeletePhotos")]
        [HttpPost]
        public void Post([FromBody]List<int> photos)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;


            var repo = _photoRepository;

            repo.DeletePhotos(photos);
        }

        // GET: api/PhotographerApi/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        //[HttpGet("{query}")]
        //[Route("SalesHistory")]
        //public async Task<IActionResult> SalesHistory(string query)
        //{
        //    var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var userId = _context.UserIdentity.Find(azureId).UserID;
        //    var user = _context.User.Find(userId);

        //    List<Order> queriedOrders;
        //    if(null == query || query.Equals(""))
        //        queriedOrders = new OrderRepository(_context).GetOrders(user.Id);
        //    else
        //        queriedOrders = new OrderRepository(_context).SearchOrders(user.Id, query);

        //    var viewModel = SalesHistoryViewModel.ToViewModel(queriedOrders);

        //    var result = await _viewRenderService.RenderToStringAsync("Photographer/Partials/_SalesHistoryPartial", viewModel);

        //    return Ok(result);
        //}
    }
}
