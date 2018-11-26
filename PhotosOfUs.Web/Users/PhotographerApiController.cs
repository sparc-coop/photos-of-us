﻿using System.Collections.Generic;
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
        private IRepository<Photo> _photos;
        private IRepository<Event> _events;
        private IRepository<User> _users;
        private IRepository<Tag> _tags;
        private readonly IRepository<Card> _card;

        public PhotographerApiController(IRepository<Photo> photoRepository, IRepository<User> userRepository, IRepository<Tag> tagRepository
            ,IRepository<Event> brandAccount, IRepository<Card> cardRepository)
        {
            _photos = photoRepository;
            _users = userRepository;
            _tags = tagRepository;
            _events = brandAccount;
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
                Photo photo = _photos.Find(x => x.Id == id);
                photoList.Add(photo);
            }

            foreach (Photo photo in photoList)
            {
                var tagsfromphoto = _photos.Include(x => x.Tag).Find(x => x.Id == photo.Id).PhotoTag;

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
            var photo = _photos.Find(x => x.Id == photoId);

            return photo.Price;
        }

        [HttpPost]
        [Route("SavePhotoPrice/{photoId:int}/{newPrice:decimal}")]
        public PhotoViewModel SavePhotoPrice(int photoId, decimal newPrice)
        {
            Photo photo = _photos.Find(x => x.Id == photoId);
            photo.UpdatePrice(newPrice);
            _photos.Commit();

            return photo.ToViewModel<PhotoViewModel>();
        }

        [HttpPost]
        [Route("AddTags")]
        public void AddTags([FromBody]List<TagViewModel> tags)
        {
            foreach (TagViewModel tag in tags)
            {
                if (_tags.Find(x => x.Name == tag.Name) == null)
                {
                    _tags.Add(TagViewModel.ToEntity(tag));
                }
            }
            _tags.Commit();
        }

        [HttpPost]
        [Route("EditPhotos")]
        public void EditTags([FromBody]PhotoTagViewModel photosviewmodel)
        {
            List<PhotoTag> phototagstodelete = new List<PhotoTag>();
            List<PhotoTag> phototagstoadd = new List<PhotoTag>();

            foreach (int photoid in photosviewmodel.photos)
            {

                var phototagdelete = _photos.Find(x => x.Id == photoid).PhotoTag;

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
                _photos.Find(x => x.Id == phototag.PhotoId).PhotoTag.Remove(phototag);
            }
            _photos.Commit();

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
        public void DeletePhotos([FromBody]List<int> photos)
        {
            var photographerId = User.ID();

            foreach(int photoId in photos)
            {
                Photo photo =_photos.Find(x => x.Id == photoId);
                _photos.Delete(photo);
            }
        }

        [Route("GetBrandSettings")]
        public Event GetBrandSettings()
        {
            var brandSettings = _events.Find(x => x.UserId == User.ID());

            if (brandSettings == null)
            {
                brandSettings = new Event().CreateDefaultBrandAccount(User.ID());
            }

            return brandSettings;
        }

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
            return _photos.Where(x => x.PublicProfile && !x.IsDeleted && x.PhotographerId == photographerId).ToList();
        }
    }
}
