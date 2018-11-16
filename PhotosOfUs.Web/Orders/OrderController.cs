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
    //[Area("Orders")]
    [Authorize]
    public class OrdersController : Controller
    {
        private IRepository<Order> _orders;

        public OrdersController(IRepository<Order> orderRepository)
        {
            _orders = orderRepository;
        }

        public ActionResult Index()
        {
            Order order = _orders.Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").FirstOrDefault();
            return View(order.ToViewModel<CustomerOrderViewModel>());
        }

        public ActionResult OrderHistory()
        {
            List<Order> orders = _orders.Include("OrderDetail.Photo").Where(x => x.UserId == User.ID()).ToList();
            return View(orders.ToViewModel<List<CustomerOrderViewModel>>());
        }

        public ActionResult Confirmation()
        {
            List<Order> orders = _orders.Where(x => x.UserId == User.ID()).ToList();
            return View(orders.ToViewModel<List<CustomerOrderViewModel>>());
        }
    }
}
