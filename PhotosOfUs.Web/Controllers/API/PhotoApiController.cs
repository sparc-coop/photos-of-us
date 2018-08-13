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

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Photo")]
    public class PhotoApiController : Controller
    {
        private PhotosOfUsContext _context;

        public PhotoApiController(PhotosOfUsContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("{photoId:int}")]
        public PhotoViewModel GetPhoto(int photoId)
        {
            var photo = new PhotoRepository(_context).GetPhoto(photoId);
            return PhotoViewModel.ToViewModel(photo);
        }

        [HttpGet]
        [Route("GetPublicIds")]
        public List<int> GetPublicIds()
        {
            var photos = new PhotoRepository(_context).GetPublicPhotos();

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
            List<Photo> photo = new PhotoRepository(_context).GetPhotosByCode(code);
            return PhotoViewModel.ToViewModel(photo).ToList();
        }

        [HttpGet]
        [Route("GetPrintTypes")]
        public List<PrintTypeViewModel> GetPrintTypes()
        {
            var printType = new PrintRepository(_context).GetPrintTypes();
            return PrintTypeViewModel.ToViewModel(printType);
        }

        [HttpGet]
        [Route("GetPhotographer/{id:int}")]
        public UserViewModel GetPhotographer(int id)
        {
            var photographer = new UserRepository(_context).Find(id);
            return UserViewModel.ToViewModel(photographer);
        }

        [HttpGet]
        [Route("GetFolders")]
        public List<FolderViewModel> GetFolders()
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;
            var folders = new PhotoRepository(_context).GetFolders(photographerId);

            return FolderViewModel.ToViewModel(folders);
        }

        [HttpGet]
        [Route("GetOrderPhotos/{id:int}")]
        public List<CustomerOrderViewModel> GetOrderPhotos(int id)
        {
            List<Order> orders = new OrderRepository(_context).GetUserOrders(id);
            return CustomerOrderViewModel.ToViewModel(orders).ToList();
        }

        [HttpGet]
        [Route("GetOrderItems/{id:int}")]
        public List<OrderDetailViewModel> GetOrderItems(int id)
        {
            return new OrderRepository(_context)
                .GetOrderDetails(id)
                .Select(x => OrderDetailViewModel.ToViewModel(x))
                .ToList();
        }

        [HttpPost]
        [Route("GetForDownload/{id:int}")]
        public IActionResult GetForDownload(int id)
        {
            Order order = new OrderRepository(_context).GetOpenOrder(id);
            List<OrderDetail> items = new OrderRepository(_context).GetOrderDetails(order.Id);

            //DownloadPhotos(items);

            return Ok();
        }

        [HttpGet]
        [Route("GetAllTags")]
        public List<TagViewModel> GetAllTags()
        {
            var tags = new PhotoRepository(_context).GetAllTags();

            return TagViewModel.ToViewModel(tags);
            //return tags;
        }
    }
}
