using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using Stripe;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PhotosOfUs.Web.Controllers
{
    [Authorize]
    public class PhotoController : Controller
    {
        private PhotosOfUsContext _context;

        public PhotoController(PhotosOfUsContext context)
        {
            _context = context;
        }

        public ActionResult Purchase(int id)
        {
            var photo = new PhotoRepository(_context).GetPhoto(id);

            var viewModel = PhotoViewModel.ToViewModel(photo);

            return View(viewModel);
        }

        public ActionResult Cart(int id)
        {
            Order order = new OrderRepository(_context).GetOpenOrder(id);
            return View(CustomerOrderViewModel.ToViewModel(order));
        }

        public ActionResult Checkout(int id)
        {
            Order order = new OrderRepository(_context).GetOpenOrder(id);
            return View(CustomerOrderViewModel.ToViewModel(order));
        }

        [HttpPost]
        public IActionResult Charge(string stripeToken)
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _context.UserIdentity.Find(azureId).UserID;
            StripeConfiguration.SetApiKey("");

            OrderRepository repo = new OrderRepository(_context);
            Order order = repo.GetOpenOrder(userId);
            repo.OrderStatusPending(order.Id);
            
            decimal orderTotal = new OrderRepository(_context).GetOrderTotal(order.Id);

            User user = new UserRepository(_context).Find(userId);
            Address address = new UserRepository(_context).GetAddress(userId);

            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();

            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = address.Email,
                SourceToken = stripeToken
            });

            var charge = charges.Create(new StripeChargeCreateOptions
            {
                Amount = (int)(orderTotal * 100),
                Description = "Photos Of Us Order",
                Currency = "usd",
                CustomerId = customer.Id,
            });

            return Redirect("/Customer/Confirmation");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        public IActionResult SaveAddress()
        {
            return View();
        }

    }
}