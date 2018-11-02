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
    [Area("Orders")]
    public class OrdersController : Controller
    {
        private IRepository<Order> _orders;
        private IRepository<Photo> _photos;
        private IRepository<User> _users;

        public OrdersController(IRepository<Order> orderRepository, IRepository<Photo> photoRepository, IRepository<User> userRepository)
        {
            _orders = orderRepository;
            _photos = photoRepository;
            _users = userRepository;
        }

        public ActionResult Index()
        {
            Order order = _orders.Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").FirstOrDefault();
            return View(order.ToViewModel<CustomerOrderViewModel>());
        }

        public ActionResult OrderHistory(int id)
        {
            List<Order> orders = _orders.Where(x => x.UserId == User.ID()).ToList();
            return View(orders.ToViewModel<List<CustomerOrderViewModel>>());
        }

        public ActionResult Confirmation()
        {
            List<Order> orders = _orders.Where(x => x.UserId == User.ID()).ToList();
            return View(orders.ToViewModel<List<CustomerOrderViewModel>>());
        }

        public ActionResult Cart(int id)
        {
            Order order = _orders.Find(x => x.Id == id);
            return View(order.ToViewModel<CustomerOrderViewModel>());
        }

        public ActionResult Purchase(int id)
        {
            var photo = _photos.Find(x => x.Id == id);
            return View(photo.ToViewModel<PhotoViewModel>());
        }

        [Authorize]
        public ActionResult Checkout()
        {
            //Order order = _orders.Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").FirstOrDefault();
            var azureId = User.AzureID();
            var user = _users.Include(x => x.UserIdentities).Find(x => x.UserIdentities.Any(y => y.AzureID == azureId));
            Order order = _orders.Where(x => x.UserId == user.Id && x.OrderStatus == "Open").FirstOrDefault();
            return View(order.ToViewModel<CustomerOrderViewModel>());
        }

    }
}
