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
            return View(CustomerOrderViewModel.ToViewModel(order));
        }

        public ActionResult Purchase(int id)
        {
            var photo = _photos.Find(x => x.Id == id);
            var viewModel = PhotoViewModel.ToViewModel(photo);
            return View(viewModel);
        }

        public ActionResult OrderHistory(int id)
        {
            List<Order> orders = _orders.Where(x => x.UserId == User.ID()).ToList();
            return View(CustomerOrderViewModel.ToViewModel(orders));
        }

        public ActionResult Confirmation()
        {
            List<Order> orders = _orders.Where(x => x.UserId == User.ID()).ToList();
            return View(CustomerOrderViewModel.ToViewModel(orders));
        }

        public ActionResult Cart()
        {
            int id = 2;
            //Order order = new OrderRepository(_context).GetOpenOrder(id);
            Order order = _orders.Find(x => x.Id == id);
            return View(CustomerOrderViewModel.ToViewModel(order));
        }
    }
}
