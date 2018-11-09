using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Web.Utilities;
using Kuvio.Kernel.Auth;
using Kuvio.Kernel.Architecture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PhotosOfUs.Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Photographer")]
    public class PhotographerApiController : Controller
    {
        private PhotosOfUsContext _context;
        private IRepository<Photo> _photo;
        private IRepository<User> _user;
        private IRepository<Tag> _tag;
        private readonly IRepository<Card> _card;

        public PhotographerApiController(PhotosOfUsContext context, IRepository<Photo> photoRepository, IRepository<User> userRepository, 
            IRepository<Tag> tagRepository, IRepository<Card> cardRepository)
        {
            _context = context;
            _photo = photoRepository;
            _user = userRepository;
            _tag = tagRepository;
            _card = cardRepository;
        }
       
        [HttpPost]
        [Route("GetTagsByPhotos")]
        public List<TagViewModel> GetTagsByPhotos([FromBody]List<int> photos)
        {
            var tags = new List<Tag>();
            var phototags = new List<PhotoTag>();

            var photoList = new List<Photo>();
            foreach (int id in photos)
            {
                Photo photo = _photo.Find(x => x.Id == id);
                photoList.Add(photo);
            }

            foreach (Photo photo in photoList)
            {
                var tagsfromphoto = _photo.Include(x => x.Tag).Find(x => x.Id == photo.Id).PhotoTag;

                foreach (PhotoTag phototag in tagsfromphoto)
                {
                    if (!(tags.Find(x => x.Id == phototag.PhotoId).Id == phototag.PhotoId))
                    {
                        phototags.Add(phototag);
                    }
                }
            }

            return TagViewModel.ToViewModel(phototags);
        }

        [HttpGet]
        [Route("GetPhotoPrice/{photoId:int}")]
        public decimal? GetPhotoPrice(int photoId)
        {
            var photo = _photo.Find(x => x.Id == photoId);

            return photo.Price;
        }

        [HttpPost]
        [Route("SavePhotoPrice/{photoId:int}/{newPrice:decimal}")]
        public PhotoViewModel SavePhotoPrice(int photoId, decimal newPrice)
        {
            Photo photo = _photo.Find(x => x.Id == photoId);
            photo.UpdatePrice(newPrice);
            _photo.Commit();

            return photo.ToViewModel<PhotoViewModel>();
        }

        [HttpPost]
        [Route("AddTags")]
        public void AddTags([FromBody]List<TagViewModel> tags)
        {
            foreach (TagViewModel tag in tags)
            {
                if (_tag.Find(x => x.Name == tag.Name) == null)
                {
                    _tag.Add(TagViewModel.ToEntity(tag));
                }
            }
            _tag.Commit();
        }

        [HttpPost]
        [Route("EditPhotos")]
        public void EditTags([FromBody]PhotoTagViewModel photosviewmodel)
        {
            List<PhotoTag> phototagstodelete = new List<PhotoTag>();
            List<PhotoTag> phototagstoadd = new List<PhotoTag>();

            foreach (int photoid in photosviewmodel.photos)
            {

                var phototagdelete = _photo.Find(x => x.Id == photoid).PhotoTag;

                if (phototagdelete != null)
                {
                    foreach (PhotoTag phototag in phototagdelete)
                    {
                        phototagstodelete.Add(phototag);
                    }

                    
                }
            }

            foreach (PhotoTag phototag in phototagstodelete)
            {
                _photo.Find(x => x.Id == phototag.PhotoId).PhotoTag.Remove(phototag);
            }
            _photo.Commit();

/*             foreach (TagViewModel tag in photoviewmodel)
            {
                var tagtoid = 

                foreach (int photoid in phototagviewmodel.photos)
                {
                    var newphototag = new PhotoTag()
                    {
                        PhotoId = photoid,
                        TagId = tagtoid.Id,
                        RegisterDate = DateTime.Now
                    };
                    PhotoTag.Add(newphototag);
                }
            }
           _photo.Commit(); */
        }

        [Route("DeletePhotos")]
        [HttpPost]
        public void Post([FromBody]List<int> photos)
        {
            var photographerId = User.ID();

            foreach(int photoId in photos)
            {
                Photo photo =_photo.Find(x => x.Id == photoId);
                _photo.Delete(photo);
            }
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

        [Authorize]
        [HttpGet]
        [Route("GetUserCard/{userId:int}")]
        public List<Card> GetUserCard(int userId)
        {
            List<Card> pCards = _card.Where(x => x.PhotographerId == userId).Include(x => x.Photographer).ToList();
            return pCards.ToList();
        }


        [HttpGet]
        [Route("GetProfilePhotos/{photographerId:int}")]
        public List<Photo> GetProfilePhotos(int photographerId)
        {
            return _photo.Where(x => x.PublicProfile && !x.IsDeleted && x.PhotographerId == photographerId).ToList();
        }
    }
}
