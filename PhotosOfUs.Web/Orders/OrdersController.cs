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
using Stripe;

namespace PhotosOfUs.Web.Controllers.API
{
    [Authorize]
    public class OrdersController : Controller
    {
        private IRepository<Order> _orders;

        public OrdersController(IRepository<Order> orderRepository)
        {
            _orders = orderRepository;
        }

        public ActionResult Confirmation()
        {
            List<Order> orders = _orders.Where(x => x.UserId == User.ID()).ToList();
            return View(orders.ToViewModel<List<CustomerOrderViewModel>>());
        }

        [HttpPost]
        public IActionResult Charge(string stripeToken)
        {
            StripeConfiguration.SetApiKey("");

            Order userOrder = _orders.Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").FirstOrDefault();
            userOrder.SetStatusToPending();
            _orders.Commit();

            Order order = _orders.Find(x => x.Id == userOrder.Id);
            decimal total = 0;
            foreach (var item in order.OrderDetail)
            {
                total += (item.UnitPrice * item.Quantity);
            }

            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();

            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = order.User.Email,
                SourceToken = stripeToken
            });

            var charge = charges.Create(new StripeChargeCreateOptions
            {
                Amount = (int)(total * 100),
                Description = "Photos Of Us Order",
                Currency = "usd",
                CustomerId = customer.Id,
            });

            return Redirect("/Orders/Confirmation");
        }

        public ActionResult Cart(int id)
        {
            Order order = _orders.Find(x => x.Id == id);
            return View(order.ToViewModel<CustomerOrderViewModel>());
        }
    }
}
