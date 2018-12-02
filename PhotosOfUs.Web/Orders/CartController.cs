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
        private IRepository<User> _user;

        public Order Cart { get; set; }

        public CartApiController(IRepository<Order> orderRepository, IRepository<User> userRepository)
        {
            _order = orderRepository;
            _user = userRepository;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            Cart = _order.Include(x => x.OrderDetail).Find(x => x.UserId == User.ID() && x.OrderStatus == "Open");
        }
       
        [HttpPost]
        [Route("SaveAddress")]
        public AddressViewModel SaveAddress([FromBody]Address vm)
        {
            var user = _user.Find(x => x.Id == User.ID());
            //var address = vm.ToViewModel<Address>();
            user.SetAddress(vm);
            _user.Commit();
            return vm.ToViewModel<AddressViewModel>();
        }

        public class OrderItemsViewModel
        {
            public int PrintTypeId { get; set; }
            public int Quantity { get; set; }
            public PrintType PrintType { get; set; }
        }
    }
}
