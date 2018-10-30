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

        [HttpGet, HttpPost]
        [Route("SaveAddress")]
        public AddressViewModel SaveAddress([FromBody]Address vm)
        {
            var user = _users.Find(x => x.Id == User.ID());
            //var address = AddressViewModel.ToEntity(vm);
            user.SetAddress(vm);
            _users.Commit();

            return vm.ToViewModel<AddressViewModel>();
        }

        [HttpGet]
        [Route("GetOrderTotal/{orderId:int}")]
        public decimal GetOrderTotal(int orderId)
        {
            Order order = _orders.Find(x => x.Id == orderId);
            decimal total = 0;
            foreach (var item in order.OrderDetail)
            {
                total += (item.UnitPrice * item.Quantity);
            }

            return total;
        }

        [HttpGet]
        [Route("GetOrders/{userId:int}")]
        public List<CustomerOrderViewModel> GetOrders(int userId)
        {
            List<Order> orders = _orders.Where(x => x.UserId == userId).ToList();
            return orders.ToViewModel<List<CustomerOrderViewModel>>();
        }

        public class OrderItemsViewModel
        {
            public int PrintTypeId { get; set; }
            public int Quantity { get; set; }
        }

        [HttpPost]
        [Route("ConfirmationEmail")]
        public async Task<string> SendConfirmationEmail([FromBody]AddressViewModel address)
        {
            var order = _orders.Include(x => x.OrderDetail).Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").FirstOrDefault();

            var apiKey = "";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("photosofus@kuviocreative.com");
            var subject = $"Photos Of Us Order Confirmation";
            var to = new EmailAddress(address.Email);
            var plainTextContent = address.FullName + ", thank you for your order.";
            var htmlContent = $"Hello {address.FullName}, <br/> Thank you for your order of {order.OrderDetail.Count()} photo(s).<br/>" +
                $"Shipping Address: <br/>{address.Address1} <br/>{address.City}, {address.State} {address.ZipCode}";
            //  $"<br/> {item.Photo.Name},{item.PrintType.Type}: {item.PrintType.Length} x {item.PrintType.Height}";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);

            return "success";
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

        public ActionResult Cart()
        {
            int id = 2;
            //Order order = new OrderRepository(_context).GetOpenOrder(id);
            Order order = _orders.Find(x => x.Id == id);
            return View(CustomerOrderViewModel.ToViewModel(order));
        }
    }
}
