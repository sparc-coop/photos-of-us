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
    [Route("api/Orders")]
    public class OrdersApiController : Controller
    {
        private PhotosOfUsContext _context;
        private IRepository<Order> _order;
        private IRepository<Photo> _photo;
        private IRepository<User> _user;

        public OrdersApiController(PhotosOfUsContext context, IRepository<Order> orderRepository, IRepository<Photo> photoRepository, IRepository<User> userRepository)
        {
            _context = context;
            _order = orderRepository;
            _photo = photoRepository;
            _user = userRepository;
        }


        [HttpGet]
        [Route("GetOrderDetails/{orderId:int}")]
        public List<Order> GetOrderDetails(int orderId)
        {
            return _order.Include(x => x.OrderDetail).Where(x => x.Id == orderId).ToList();
        }

        [HttpGet, HttpPost]
        [Route("SaveAddress")]
        public AddressViewModel SaveAddress([FromBody]AddressViewModel vm)
        {
            var user = _user.Find(x => x.Id == User.ID());

            var address = AddressViewModel.ToEntity(vm);
            user.SetAddress(address);
            _context.SaveChanges();
            return AddressViewModel.ToViewModel(address);
        }

        [HttpGet]
        [Route("GetOrderTotal/{orderId:int}")]
        public decimal GetOrderTotal(int orderId)
        {
            Order order = _order.Find(x => x.Id == orderId);
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
            List<Order> orders = _order.Where(x => x.UserId == userId).ToList();
            return CustomerOrderViewModel.ToViewModel(orders).ToList();
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
            var order = _context.Orders.Include("OrderDetail").Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").FirstOrDefault();

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
    }
}
