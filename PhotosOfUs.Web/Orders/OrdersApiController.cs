using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Security.Claims;
using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Web.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Orders")]
    [Authorize]
    public class OrdersApiController : Controller
    {
        private IRepository<Order> _orders;
        private IRepository<OrderDetail> _orderDetail;
        private IRepository<Photo> _photos;
        private IRepository<User> _users;

        public OrdersApiController(IRepository<Order> orderRepository, IRepository<OrderDetail> orderDetailRepository, IRepository<Photo> photoRepository, IRepository<User> userRepository)
        {
            _orders = orderRepository;
            _orderDetail = orderDetailRepository;
            _photos = photoRepository;
            _users = userRepository;
        }


        [HttpGet]
        [Route("GetOrderDetails/{orderId:int}")]
        public Order GetOrderDetails(int orderId)
        {
            return _orders.Include("OrderDetail.Photo").Where(x => x.Id == orderId).FirstOrDefault();
        }

        [HttpGet]
        [Route("GetSalesHistory/")]
        public object GetSalesHistory()
        {
            var photoIds = _photos.Where(x => x.PhotographerId == User.ID()).Select(x => x.Id);

            var orderDetails = _orderDetail.Include(x => x.Photo).Where(x => photoIds.Contains(x.PhotoId));

            var salesHistory = orderDetails.GroupBy(x => new { x.PhotoId, x.Photo.Name })
                .Select(x => new
                {
                    PhotoId = x.Key.PhotoId,
                    PhotoName = x.Key.Name,
                    Quantity = x.Sum(y => y.Quantity),
                    UnitPrice = x.Sum(y => y.UnitPrice),
                    Earnings = x.Sum(y => y.Photo.Price)
                }).ToList();

            return salesHistory;
        }

        [HttpGet]
        [Route("GetPrintTypes")]
        public List<PrintTypeViewModel> GetPrintTypes()
        {
            return new Photo().GetPrintTypes().ToList().ToViewModel<List<PrintTypeViewModel>>();
        }
    }
}
