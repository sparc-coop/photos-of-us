using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Kuvio.Kernel.Core;
using System.Threading.Tasks;
using PhotosOfUs.Core.Events;
using PhotosOfUs.Core.Photos;
using PhotosOfUs.Core.Photos.Commands;
using PhotosOfUs.Core.Users;

namespace PhotosOfUs.Web.Controllers.API
{
    //[Produces("application/json")]
    //[Route("api/[controller]")]
    [ApiController]
    [Route("[controller]")]
    public class PhotographerApiController : ControllerBase
    {
        private PhotosOfUsContext _context;
        private IRepository<Photo> _photo;
        private IDbRepository<PhotoTag> _photoTag;
        //private IRepository<BrandAccount> _brandAccount;
        private IRepository<User> _user;
        private IRepository<Tag> _tag;
        private readonly IDbRepository<Card> _card;

        public PhotographerApiController(PhotosOfUsContext context, IRepository<Photo> photoRepository, IRepository<User> userRepository, IRepository<Tag> tagRepository
            , IDbRepository<Card> cardRepository, IDbRepository<PhotoTag> photoTagRepository)
        {
            _context = context;
            _photo = photoRepository;
            _user = userRepository;
            _tag = tagRepository;
            _card = cardRepository;
            _photoTag = photoTagRepository;
        }
       
        [HttpPost]
        [Route("GetTagsByPhotos")]
        public List<TagViewModel> GetTagsByPhotos([FromBody]List<int> photos)
        {
            var tagList = _photoTag.Include("Tag").Where(x => photos.Contains(x.PhotoId)).Select(x => x.Tag).Distinct().ToList();
            
            return TagViewModel.ToViewModel(tagList);
        }

        [HttpGet]
        [Route("GetPhotoPrice/{photoId:int}")]
        public async Task<decimal?> GetPhotoPrice(int photoId)
        {
            var photo = await _photo.FindAsync(x => x.Id == photoId);

            return photo.Price;
        }

        [HttpPost]
        [Route("SavePhotoPrice/{photoId:int}/{newPrice:decimal}")]
        public async Task<Photo> SavePhotoPrice(int photoId, decimal newPrice)
        {
            Photo photo = await _photo.FindAsync(x => x.Id == photoId);
            photo.UpdatePrice(newPrice);
            await _photo.CommitAsync();

            return photo;
        }

        //[HttpPost]
        //[Route("EditPhotos")]
        //public async Task EditTags([FromBody]PhotoTagViewModel photoTagViewModel)
        //{
        //    _photoTag.DeleteAsync(_photoTag.Query.Where(x => photoTagViewModel.photos.Contains(x.PhotoId)).ToList());

        //    foreach (var photo in photoTagViewModel.photos)
        //    {
        //        foreach (var tag in photoTagViewModel.tags)
        //        {
        //            var newTag = AddTags(tag);
        //            await _photoTag.AddAsync(new PhotoTag (photo, newTag));
        //        }
        //    }

        //    await _photoTag.CommitAsync();
        //}

        public async Task<List<Tag>> AddTags(List<TagViewModel> tags)
        {
            tags.ForEach(x => x.Name = x.Name.Trim());
            tags = tags.Distinct().ToList();

            List<Tag> resultTags = new List<Tag>();

            var existingTags = _tag.Query.Where(x => tags.Select(y => y.Name).Contains(x.Name)).ToList();

            foreach (TagViewModel item in tags)
            {
                if (!existingTags.Select(y => y.Name).Contains(item.Name))
                {
                    await _tag.AddAsync(new Tag(item.Name));
                    //resultTags.Add(newTag);
                }
            }
            await _tag.CommitAsync();
            //return resultTags;
            return null;
        }

        public async Task<Tag> AddTag(TagViewModel tag)
        {
            tag.Name = tag.Text.Trim();

            var newTag = await _tag.FindAsync(x => x.Name == tag.Name);

            if (newTag == null)
            {
                newTag = new Tag(tag.Name);
                await _tag.AddAsync(newTag);
                await _tag.CommitAsync();
            }

            return newTag;
        }

        [Route("DeletePhotos")]
        [HttpPost]
        public async Task Post([FromBody]List<int> photosIds)
        {
            var photographerId = User.Id();

            var photos = _photo.Query.Where(x => photosIds.Contains(x.Id)).ToList();

            foreach(var photo in photos)
            {
                await _photo.DeleteAsync(photo);
            }

            await _photo.CommitAsync();
        }

        //[Route("GetBrandSettings")]
        //public BrandAccount GetBrandSettings()
        //{
        //    var brandSettings = _brandAccount.Find(x => x.UserId == User.Id());

        //    if (brandSettings == null)
        //    {
        //        brandSettings = new BrandAccount().CreateDefaultBrandAccount(User.Id());
        //    }

        //    return brandSettings;
        //}

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

        //[Authorize]
        //[HttpGet]
        //[Route("GetUserCard/{userId:int}")]
        //public List<Card> GetUserCard(int userId)
        //{
        //    List<Card> pCards = _user.Query.Where(x => x.Id == userId).Select(y => y)
        //    return pCards.ToList();
        //}


        [HttpGet]
        [Route("GetProfilePhotos/{photographerId:int}")]
        public List<Photo> GetProfilePhotos(int photographerId)
        {
            return _photo.Query.Where(x => x.PublicProfile && !x.IsDeleted && x.PhotographerId == photographerId).ToList();
        }
    }
}
