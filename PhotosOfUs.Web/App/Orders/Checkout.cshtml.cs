using Kuvio.Kernel.Core;
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
using Order = PhotosOfUs.Model.Models.Order;
using Address = PhotosOfUs.Model.Models.Address;

namespace PhotosOfUs.Pages.Orders
{
    public class CheckoutModel : PageModel
    {
        private IRepository<Order> _orders;
        private readonly IRepository<User> _users;

        public Order Order { get; private set; }

        public CheckoutModel(IRepository<Order> orders, IRepository<User> users)
        {
            _orders = orders;
            _users = users;
        }

        public void OnGet()
        {
            Order = _orders.Include("OrderDetail.Photo").Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").FirstOrDefault();
        }

        public async Task<IActionResult> OnPostAsync(Address address, string stripeToken)
        {
            SaveAddress(address);
            ChargePayment(stripeToken);
            await SendConfirmationEmail();

            return RedirectToPage("Confirmation");
        }

        public void SaveAddress(Address address)
        {
            var user = _users.Find(x => x.Id == User.ID());
            user.SetAddress(address);
            _users.Commit();
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

            var customers = new Stripe.CustomerService();
            var charges = new Stripe.ChargeService();

            var customer = customers.Create(new Stripe.CustomerCreateOptions
            {
                Email = order.User.Email,
                SourceToken = stripeToken
            });

            var charge = charges.Create(new Stripe.ChargeCreateOptions
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