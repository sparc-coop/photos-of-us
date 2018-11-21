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
using Stripe;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Photo")]
    public class PhotoApiController : Controller
    {
        private readonly IRepository<Photo> _photos;
        private readonly IRepository<User> _users;
        private readonly IRepository<Order> _orders;
        private readonly IRepository<PrintType> _printType;
        private readonly IRepository<Tag> _tag;

        public PhotoApiController(IRepository<Photo> photoRepository, IRepository<User> userRepository, IRepository<PrintType> printTypeRepository, IRepository<Tag> tagRepository)
        {
            _photos = photoRepository;
            _users = userRepository;
            _printType = printTypeRepository;
            _tag = tagRepository;
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
            return _photos.Find(x => x.Id == photoId).ToViewModel<PhotoViewModel>();
        }

        [HttpGet]
        [Route("GetPublicPhotos")]
        public List<Photo> GetPublicPhotos()
        {
            return _photos.Where(x => x.PublicProfile && !x.IsDeleted).ToList();
        }

        [HttpGet]
        [Route("GetPublicIds")]
        public List<int> GetPublicIds()
        {
            var photos = _photos.Where(x => x.PublicProfile && !x.IsDeleted).ToList();

            List<int> photoIds = new List<int>();
            foreach (var photo in photos)
            {
                photoIds.Add(photo.Id);
            }

            return photoIds;
        }

        [HttpGet]
        [Route("GetCodePhotos/{code}")]
        public List<PhotoViewModel> GetCodePhotos(string code)
        {
            return _photos.Where(x => x.Code == code).ToViewModel<PhotoViewModel>().ToList();
        }

        [HttpGet]
        [Route("GetPrintTypes")]
        public List<PrintTypeViewModel> GetPrintTypes()
        {
            return new Photo().GetPrintTypes().ToList().ToViewModel<List<PrintTypeViewModel>>();
        }

        [HttpGet]
        [Route("GetAllTags")]
        public List<TagViewModel> GetAllTags()
        {
            return _tag.ToViewModel<List<TagViewModel>>();
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
                var hasTags = _photos.Where(x => x.Tag.Any(o => o.Name == tag.text));
                if (hasTags.Count() > 0)
                {
                    photo.NewTag(TagViewModel.ToEntity(tag));
                }
            }
            _photos.Commit();
        }

        [HttpDelete]
        public void DeletePhotos(List<int> photos)
        {
            foreach (int photoId in photos)
            {
                var photo = _photos.Find(x => x.Id == photoId);
                photo.Delete();
                _photos.Commit();
            }
        }

        public ActionResult Purchase(int id)
        {
            var photo = _photos.Find(x => x.Id == id);
            var viewModel = photo.ToViewModel<PhotoViewModel>();

            return View(viewModel);
        }

        public ActionResult Cart(int id)
        {
            Order order = _orders.Where(x => x.UserId == id && x.OrderStatus == "Open").FirstOrDefault();
            return View(order.ToViewModel<CustomerOrderViewModel>());
        }

        public ActionResult Checkout(int id)
        {
            Order order = _orders.Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").FirstOrDefault();
            return View(order.ToViewModel<CustomerOrderViewModel>());
        }

        [HttpPost]
        public IActionResult Charge(string stripeToken)
        {
            StripeConfiguration.SetApiKey("");

            Order userOrder = _orders.Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").FirstOrDefault();
            userOrder.SetStatusToPending();
            _orders.Commit();

            Order order = _orders.Find(x => x.Id == userOrder.Id);
            decimal total = 0;
            foreach (var item in order.OrderDetail)
            {
                total += (item.UnitPrice * item.Quantity);
            }

            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();

            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = order.User.Email,
                SourceToken = stripeToken
            });

            var charge = charges.Create(new StripeChargeCreateOptions
            {
                Amount = (int)(total * 100),
                Description = "Photos Of Us Order",
                Currency = "usd",
                CustomerId = customer.Id,
            });

            return Redirect("/Customer/Confirmation");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        public IActionResult SaveAddress()
        {
            return View();
        }
    }
}
