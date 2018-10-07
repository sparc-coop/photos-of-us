using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;
using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Web.Utilities;
using PhotosOfUs.Model.Photos.Commands;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Photo")]
    public class PhotoApiController : Controller
    {
        private PhotosOfUsContext _context;
        private readonly IRepository<Photo> _photo;
        private readonly IRepository<User> _user;
        private readonly IRepository<PrintType> _printType;

        public PhotoApiController(PhotosOfUsContext context, IRepository<Photo> photoRepository, IRepository<User> userRepository, IRepository<PrintType> printTypeRepository)
        {
            _context = context;
            _photo = photoRepository;
            _user = userRepository;
            _printType = printTypeRepository;
        }

        [HttpPut]
        public async Task<AzureCognitiveViewModel> Put(int userId, string photoCode, string tags, string photoName, string extension, int folderId, int price,
         [FromBody]IFormFile file, [FromServices]UploadPhotoCommand command)
        {
/*             if (User.ID() != userId)
                return Forbid(); */

            RootObject tagsfromazure = null;

            var ac = new AzureCognitive();
            var imgbytes = AzureCognitive.TransformImageIntoBytes(file);
            tagsfromazure = await ac.MakeRequest(imgbytes, "tags");

            if (string.IsNullOrEmpty(photoCode))
            {
                var codefromazure = await ac.MakeRequest(imgbytes, "ocr");

                var suggestedtags = ac.ExtractTags(tagsfromazure);
                var code = ac.ExtractCardCode(codefromazure);

                return AzureCognitiveViewModel.ToViewModel(code, suggestedtags);
            }

            var listoftags = new List<TagViewModel>();
            if (tags != null)
            {
                List<string> result = tags.Split(' ').ToList();

                foreach (string obj in result)
                {
                    listoftags.Add(new TagViewModel() { Name = obj, text = obj });
                }
            }

            var filePath = Path.GetTempFileName();

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    await command.ExecuteAsync(userId, stream, photoName, photoCode, extension, folderId, price, tagsfromazure, TagViewModel.ToEntity(listoftags));
                }
            }

            

            //return Ok();
            return new AzureCognitiveViewModel();
        }

        [HttpGet]
        [Route("{photoId:int}")]
        public PhotoViewModel GetPhoto(int photoId)
        {
            return _photo.Find(x => x.Id == photoId).ToViewModel<PhotoViewModel>();
        }

        [HttpGet]
        [Route("GetPublicPhotos")]
        public List<Photo> GetPublicPhotos()
        {
            return _photo.Where(x => x.PublicProfile && !x.IsDeleted).ToList();
        }

        [HttpGet]
        [Route("GetProfilePhotos")]
        public List<Photo> GetProfilePhotos(int photographerId)
        {
            return _photo.Where(x => x.PublicProfile && !x.IsDeleted && x.PhotographerId == photographerId).ToList();
        }

        [HttpGet]
        [Route("GetPublicIds")]
        public List<int> GetPublicIds()
        {
            var photos = _photo.Where(x => x.PublicProfile && !x.IsDeleted).ToList();

            List<int> photoIds = new List<int>();
            foreach(var photo in photos)
            {
                photoIds.Add(photo.Id);
            }

            return photoIds;
        }

        [HttpGet]
        [Route("GetCodePhotos/{code}")]
        public List<PhotoViewModel> GetCodePhotos(string code)
        {
            return _photo.Where(x => x.Code == code).ToViewModel<PhotoViewModel>().ToList();
        }

        [HttpGet]
        [Route("GetPrintTypes")]
        public List<PrintTypeViewModel> GetPrintTypes()
        {
            return PrintTypeViewModel.ToViewModel(new Photo().GetPrintTypes().ToList());
        }

        [HttpGet]
        [Route("GetAllTags")]
        public List<TagViewModel> GetAllTags()
        {
            return TagViewModel.ToViewModel(new Photo().GetAllTags().ToList());
        }

        [HttpGet]
        [Route("GetTags")]
        public List<TagViewModel> GetTags(string tagnames)
        {
            return TagViewModel.ToViewModel(new Photo().GetAllTags().ToList());
        }

        public void AddTags(List<TagViewModel> tags)
        {          
            Photo photo = new Photo();
            foreach (TagViewModel tag in tags)
            {
                var hasTags = _photo.Where(x => x.Tag.Any(o => o.Name == tag.text));
                if (hasTags.Count() > 0)
                {
                    photo.NewTag(TagViewModel.ToEntity(tag));
                }
            }
            _photo.Commit();
        }

        [HttpDelete]
        public void DeletePhotos(List<int> photos)
        {
            foreach (int photoId in photos)
            {
                var photo = _photo.Find(x => x.Id == photoId);
                photo.Delete();
                _photo.Commit();
            }          
        }
    }
}
