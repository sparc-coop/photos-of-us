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
    public class OrdersController : Controller
    {
        private IRepository<Order> _orders;
        private IRepository<OrderDetail> _orderDetail;
        private IRepository<Photo> _photos;

        public OrdersController(IRepository<Order> orderRepository, IRepository<OrderDetail> orderDetailRepository, IRepository<Photo> photoRepository)
        {
            _orders = orderRepository;
            _orderDetail = orderDetailRepository;
            _photos = photoRepository;
        }

        [HttpGet]
        [Route("GetOrderDetails/{orderId:int}")]
        public Order GetOrderDetails(int orderId)
        {
            return _orders.Include("OrderDetail.Photo").Where(x => x.Id == orderId).FirstOrDefault();
        }
    }
}
