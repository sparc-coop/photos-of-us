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

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Cart")]
    public class CartApiController : Controller
    {
        private PhotosOfUsContext _context;
        private OrderRepository _oldOrderRepository;
        private IRepository<Order> _orders;
        private PhotoRepository _photoRepository;

        public Order Cart { get; set; }

        public CartApiController(PhotosOfUsContext context, IRepository<Order> orderRepository, IRepository<Photo> photoRepository)
        {
            _context = context;
            _orders = orderRepository;
            _photoRepository = photoRepository;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            Cart = _orders.Include(x => x.OrderDetail).Find(x => x.UserId == User.ID() && x.OrderStatus == "Open");
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
                var photo = _photoRepository.Find(x => x.Id == photoId);
                Cart.AddLine(photo, item.PrintType, item.Quantity);
            }

            return Cart.ToViewModel<OrderViewModel>();
        }

        [HttpGet, HttpPost]
        [Route("SaveAddress")]
        public AddressViewModel SaveAddress([FromBody]AddressViewModel vm)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.UserIdentity.Find(azureId).UserID;

            var address = AddressViewModel.ToEntity(vm);
            user.SetAddress(address);
            _context.SaveChanges();
            return AddressViewModel.ToViewModel(address);
        }

        
        [HttpGet]
        [Route("GetOrderTotal/{orderId:int}")]
        public decimal GetOrderTotal(int orderId)
        {
            return _oldOrderRepository.GetOrderTotal(orderId);
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
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _context.UserIdentity.Find(azureId).UserID;

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
