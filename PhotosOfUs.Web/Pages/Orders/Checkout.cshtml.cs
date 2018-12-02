using Kuvio.Kernel.Architecture;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Model.Models;
using System.Linq;
using System.Collections.Generic;
using Stripe;
using Microsoft.AspNetCore.Mvc;

namespace PhotosOfUs.Pages.Orders
{
    public class CheckoutModel : PageModel
    {
        private IRepository<Order> _orders;
        public int OrderId { get; private set; }

        public CheckoutModel(IRepository<Order> orders)
        {
            _orders = orders;
        }

        public void OnGet()
        {
            OrderId = _orders.Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").Select(x => x.Id).FirstOrDefault();
        }

        public IActionResult OnPost(string stripeToken)
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

            return RedirectToPage("Confirmation");
        }
    }
}