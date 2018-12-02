using Kuvio.Kernel.Architecture;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Model.Models;
using System.Linq;
using System.Collections.Generic;
using Stripe;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace PhotosOfUs.Pages.Orders
{
    public class CheckoutModel : PageModel
    {
        private IRepository<Order> _orders;
        private readonly IRepository<User> _users;

        public int OrderId { get; private set; }

        public CheckoutModel(IRepository<Order> orders, IRepository<User> users)
        {
            _orders = orders;
            _users = users;
        }

        public void OnGet()
        {
            OrderId = _orders.Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").Select(x => x.Id).FirstOrDefault();
        }

        public async Task<IActionResult> OnPostAsync(string stripeToken)
        {
            ChargePayment(stripeToken);
            await SendConfirmationEmail();

            return RedirectToPage("Confirmation");
        }

        private void ChargePayment(string stripeToken)
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
        }

        private async Task SendConfirmationEmail()
        {
            var user = _users.Find(x => x.Id == User.ID());
            var count = _orders.Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").SelectMany(x => x.OrderDetail).Count();

            var apiKey = "";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("photosofus@kuviocreative.com");
            var subject = $"Photos Of Us Order Confirmation";
            var to = new EmailAddress(user.Email);
            var plainTextContent = user.FullName + ", thank you for your order.";
            var htmlContent = $"Hello {user.FullName}, <br/> Thank you for your order of {count} photo(s).<br/>" +
                $"Shipping Address: <br/>{user.Address.Address1} <br/>{user.Address.City}, {user.Address.State} {user.Address.ZipCode}";
            //  $"<br/> {item.Photo.Name},{item.PrintType.Type}: {item.PrintType.Length} x {item.PrintType.Height}";
            
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
        }
    }
}