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
using PhotosOfUs.Web.Models;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Cart")]
    public class CartApiController : Controller
    {
        private IRepository<Order> _order;
        private IRepository<Photo> _photo;
        private IRepository<User> _user;

        public Order Cart { get; set; }

        public CartApiController(IRepository<Order> orderRepository, IRepository<Photo> photoRepository, IRepository<User> userRepository)
        {
            _order = orderRepository;
            _photo = photoRepository;
            _user = userRepository;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            Cart = _order.Include(x => x.OrderDetail).Find(x => x.UserId == User.ID() && x.OrderStatus == "Open");
        }

        [HttpGet]
        [Route("")]
        public Order GetOpenOrder(int userId)
        {
            return Cart;
        }


        [HttpPost]
        [Route("{photoId:int}")]
        public OrderViewModel AddToCart(int photoId, [FromBody]List<OrderItemsViewModel> orderitems)
        {
            if (Cart == null)
                Cart = new Order(User.ID());
            
            foreach (var item in orderitems)
            {
                var photo = _photo.Find(x => x.Id == photoId);
                Cart.AddLine(photo, item.PrintType, item.Quantity);
            }

            return Cart.ToViewModel<OrderViewModel>();
        }

        [HttpGet, HttpPost]
        [Route("SaveAddress")]
        public AddressViewModel SaveAddress([FromBody]AddressViewModel vm)
        {
            var user = _user.Find(x => x.Id == User.ID());
            var address = AddressViewModel.ToEntity(vm);
            user.SetAddress(address);
            _user.Commit();
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

        

        public class OrderItemsViewModel
        {
            public int PrintTypeId { get; set; }
            public int Quantity { get; set; }
            public PrintType PrintType { get; set; }
        }

        [HttpPost]
        [Route("ConfirmationEmail")]
        public async Task<string> SendConfirmationEmail([FromBody]AddressViewModel address)
        {
            var apiKey = "";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("photosofus@kuviocreative.com");
            var subject = $"Photos Of Us Order Confirmation";
            var to = new EmailAddress(address.Email);
            var plainTextContent = address.FullName + ", thank you for your order.";
            var htmlContent = $"Hello {address.FullName}, <br/> Thank you for your order of {Cart.OrderDetail.Count()} photo(s).<br/>" +
                $"Shipping Address: <br/>{address.Address1} <br/>{address.City}, {address.State} {address.ZipCode}";
            //  $"<br/> {item.Photo.Name},{item.PrintType.Type}: {item.PrintType.Length} x {item.PrintType.Height}";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);

            return "success";
        }
    }
}
