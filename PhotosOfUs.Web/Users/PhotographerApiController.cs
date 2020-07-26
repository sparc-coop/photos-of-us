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
        private IRepository<PhotoTag> _photoTag;
        private IRepository<BrandAccount> _brandAccount;
        private IRepository<User> _user;
        private IRepository<Tag> _tag;
        private readonly IRepository<Card> _card;

        public PhotographerApiController(PhotosOfUsContext context, IRepository<Photo> photoRepository, IRepository<User> userRepository, IRepository<Tag> tagRepository
            ,IRepository<BrandAccount> brandAccount, IRepository<Card> cardRepository, IRepository<PhotoTag> photoTagRepository)
        {
            _context = context;
            _photo = photoRepository;
            _user = userRepository;
            _tag = tagRepository;
            _brandAccount = brandAccount;
            _card = cardRepository;
            _photoTag = photoTagRepository;
        }
       
        [HttpPost]
        [Route("GetTagsByPhotos")]
        public List<TagViewModel> GetTagsByPhotos([FromBody]List<int> photos)
        {
            var tagList = _photoTag.Include(y => y.Tag).Where(x => photos.Contains(x.PhotoId)).Select(x => x.Tag).Distinct().ToList();
            
            return TagViewModel.ToViewModel(tagList);
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
        [Route("EditPhotos")]
        public void EditTags([FromBody]PhotoTagViewModel photoTagViewModel)
        {
            _photoTag.Delete(_photoTag.Where(x => photoTagViewModel.photos.Contains(x.PhotoId)).ToList());

            foreach (var photo in photoTagViewModel.photos)
            {
                foreach (var tag in photoTagViewModel.tags)
                {
                    var newTag = AddTags(tag);
                    _photoTag.Add(new PhotoTag { PhotoId = photo, TagId = newTag.Id });
                }
            }

            _photoTag.Commit();
        }

        public List<Tag> AddTags(List<TagViewModel> tags)
        {
            tags.ForEach(x => x.Name = x.Name.Trim());
            tags = tags.Distinct().ToList();

            List<Tag> resultTags = new List<Tag>();

            var existingTags = _tag.Where(x => tags.Select(y => y.Name).Contains(x.Name)).ToList();

            foreach (TagViewModel item in tags)
            {
                if (!existingTags.Select(y => y.Name).Contains(item.Name))
                {
                    var newTag = TagViewModel.ToEntity(item);
                    _tag.Add(newTag);
                    resultTags.Add(newTag);
                }
            }
            _tag.Commit();
            return resultTags;

        }

        public Tag AddTags(TagViewModel tag)
        {
            tag.Name = tag.text.Trim();

            var newTag = _tag.Find(x => x.Name == tag.Name);

            if (newTag == null)
            {
                newTag = TagViewModel.ToEntity(tag);
                _tag.Add(newTag);
            }

            return newTag;
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

        [Route("GetBrandSettings")]
        public BrandAccount GetBrandSettings()
        {
            var brandSettings = _brandAccount.Find(x => x.UserId == User.ID());

            if (brandSettings == null)
            {
                brandSettings = new BrandAccount().CreateDefaultBrandAccount(User.ID());
            }

            return brandSettings;
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
