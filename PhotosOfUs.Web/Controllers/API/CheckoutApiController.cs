using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Checkout")]
    public class CheckoutApiController
    {
        private PhotosOfUsContext _context;

        public CheckoutApiController(PhotosOfUsContext context)
        {
            _context = context;
        }

        [HttpGet, HttpPost]
        [Route("SaveAddress")]
        public AddressViewModel GetPhoto(AddressViewModel vm)
        {
            var address = AddressViewModel.ToEntity(vm);
            address = new AddressRepository(_context).Create(address);
            if(address == null)
            {
                return new AddressViewModel();
            }
            return AddressViewModel.ToViewModel(address);
        }

        [HttpGet]
        [Route("GetOrderTotal/{orderId:int}")]
        public decimal GetOrderTotal(int orderId)
        {
            return new OrderRepository(_context).GetOrderTotal(orderId);
        }

        [HttpPost]
        [Route("CreateOrder/{userId:int}/{photoId:int}")]
        public string CreateOrder(int userId, int photoId, [FromBody]int[] orderitems)
        {
            var existingOrder = new OrderRepository(_context).GetOpenOrder(userId);
            var photo = new PhotoRepository(_context).GetPhoto(photoId);

            if(existingOrder == null)
            {
                var newOrder = new OrderRepository(_context).CreateOrder(userId);

                foreach (var item in orderitems)
                {
                    var newOrderDetail = new OrderRepository(_context).CreateOrderDetails(newOrder.Id, photoId, photo.PhotographerId, item);
                }
            }
            else
            {
                foreach (var item in orderitems)
                {
                    var newOrderDetail = new OrderRepository(_context).CreateOrderDetails(existingOrder.Id, photoId, photo.PhotographerId, item);
                }
            }
            return "success";
        }
    }
}
