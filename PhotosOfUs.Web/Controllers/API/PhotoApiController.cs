using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;
using System.Security.Claims;
using System.Net;
using System.IO;
using Kuvio.Kernel.Architecture;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Photo")]
    public class PhotoApiController : Controller
    {
        private PhotosOfUsContext _context;
        private readonly PhotoRepository _photoRepository;
        private readonly OrderRepository _orderRepository;
        private readonly IRepository<User> _userRepository;

        public PhotoApiController(PhotosOfUsContext context, PhotoRepository photoRepository, OrderRepository orderRepository, IRepository<User> userRepository)
        {
            _context = context;
            _photoRepository = photoRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("{photoId:int}")]
        public PhotoViewModel GetPhoto(int photoId)
        {
            var photo = _photoRepository.GetPhoto(photoId);
            return PhotoViewModel.ToViewModel(photo);
        }

        [HttpGet]
        [Route("GetPublicIds")]
        public List<int> GetPublicIds()
        {
            var photos = _photoRepository.GetPublicPhotos();

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
            List<Photo> photo = _photoRepository.GetPhotosByCode(code);
            return PhotoViewModel.ToViewModel(photo).ToList();
        }

        [HttpGet]
        [Route("GetPrintTypes")]
        public List<PrintTypeViewModel> GetPrintTypes()
        {
            var printType = _context.PrintType.ToList();
            return PrintTypeViewModel.ToViewModel(printType);
        }

        [HttpGet]
        [Route("GetPhotographer/{id:int}")]
        public UserViewModel GetPhotographer(int id)
        {
            var photographer = _userRepository.Find(x => x.Id == id);
            return UserViewModel.ToViewModel(photographer);
        }

        [HttpGet]
        [Route("GetFolders")]
        public List<FolderViewModel> GetFolders()
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;
            var folders = _photoRepository.GetFolders(photographerId);

            return FolderViewModel.ToViewModel(folders);
        }

        [HttpGet]
        [Route("GetOrderPhotos/{id:int}")]
        public List<CustomerOrderViewModel> GetOrderPhotos(int id)
        {
            List<Order> orders = _orderRepository.GetUserOrders(id);
            return CustomerOrderViewModel.ToViewModel(orders).ToList();
        }

        [HttpGet]
        [Route("GetOrderItems/{id:int}")]
        public List<OrderDetailViewModel> GetOrderItems(int id)
        {
            return _orderRepository
                .GetOrderDetails(id)
                .Select(x => OrderDetailViewModel.ToViewModel(x))
                .ToList();
        }

        [HttpPost]
        [Route("GetForDownload/{id:int}")]
        public IActionResult GetForDownload(int id)
        {
            Order order = _orderRepository.GetOpenOrder(id);
            List<OrderDetail> items = _orderRepository.GetOrderDetails(order.Id);

            //DownloadPhotos(items);

            return Ok();
        }

        [HttpGet]
        [Route("GetAllTags")]
        public List<TagViewModel> GetAllTags()
        {
            var tags = _photoRepository.GetAllTags();

            return TagViewModel.ToViewModel(tags);
            //return tags;
        }
    }
}
