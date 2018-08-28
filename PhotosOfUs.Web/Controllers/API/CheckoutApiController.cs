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

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Checkout")]
    public class CheckoutApiController : Controller
    {
        private PhotosOfUsContext _context;

        public CheckoutApiController(PhotosOfUsContext context)
        {
            _context = context;
        }

        [HttpGet, HttpPost]
        [Route("SaveAddress")]
        public AddressViewModel SaveAddress([FromBody]AddressViewModel vm)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _context.UserIdentity.Find(azureId).UserID;

            var address = AddressViewModel.ToEntity(vm);

            address.UserId = userId;
            address = new AddressRepository(_context).Create(address);
            if(address == null)
            {
                return new AddressViewModel();
            }
            return AddressViewModel.ToViewModel(address);
        }

        [HttpGet]
        [Route("GetOpenOrder/{userId:int}")]
        public Order GetOpenOrder(int userId)
        {
            var openOrder = new OrderRepository(_context).GetOpenOrder(userId);
            if(openOrder != null)
            {
                return openOrder;
            }

            return null;
        }

        [HttpGet]
        [Route("GetOrderTotal/{orderId:int}")]
        public decimal GetOrderTotal(int orderId)
        {
            return new OrderRepository(_context).GetOrderTotal(orderId);
        }

        [HttpPost]
        [Route("CreateOrder/{userId:int}/{photoId:int}")]
        public string CreateOrder(int userId, int photoId, [FromBody]List<OrderItemsViewModel> orderitems)
        {
            var existingOrder = new OrderRepository(_context).GetOpenOrder(userId);
            var photo = new PhotoRepository(_context).GetPhoto(photoId);

            if(existingOrder == null)
            {
                var newOrder = new OrderRepository(_context).CreateOrder(userId);

                foreach (var item in orderitems)
                {
                    var newOrderDetail = new OrderRepository(_context).CreateOrderDetails(newOrder.Id, photoId, photo.PhotographerId, item.PrintTypeId, int.Parse(item.Quantity));
                }
            }
            else
            {
                foreach (var item in orderitems)
                {
                    var newOrderDetail = new OrderRepository(_context).CreateOrderDetails(existingOrder.Id, photoId, photo.PhotographerId, item.PrintTypeId, int.Parse(item.Quantity));
                }
            }
            return "success";
        }

        public class OrderItemsViewModel
        {
            public int PrintTypeId { get; set; }
            public string Quantity { get; set; }
        }

        [HttpPost]
        [Route("ConfirmationEmail")]
        public async Task<string> SendConfirmationEmail([FromBody]AddressViewModel address)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _context.UserIdentity.Find(azureId).UserID;
            var order = new OrderRepository(_context).GetOpenOrder(userId);

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

        [HttpGet]
        [Route("GetAddress/{userId:int}")]
        public Address GetAddress(int userId)
        {
            Address address = new AddressRepository(_context).FindAddress(userId);
            return address;
        }
    }
}
