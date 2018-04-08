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

namespace PhotosOfUs.Web.Controllers
{
    public class PhotoController : Controller
    {
        private PhotosOfUsContext _context;

        public PhotoController(PhotosOfUsContext context)
        {
            _context = context;
        }
        // GET: Photographer
        public ActionResult Purchase(int id)
        {
            var photo = new PhotoRepository(_context).GetPhoto(id);

            var viewModel = PhotoViewModel.ToViewModel(photo);

            return View(viewModel);
        }

        public ActionResult Cart(int id)
        {
            return View();
        }

        public ActionResult Checkout(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Charge([FromBody] PaymentModel payment)
        {
            StripeConfiguration.SetApiKey("");
            Debug.WriteLine(payment);
            int amount = payment.Amount;
            var userId = 1; // TODO: get actual user id
            User user = new UserRepository(_context).Find(userId);
            Address address = new UserRepository(_context).GetAddress(userId);

            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();

            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = address.Email,
                SourceToken = payment.StripeToken
            });

            var charge = charges.Create(new StripeChargeCreateOptions
            {
                Amount = amount,
                Description = "Sample Charge",
                Currency = "usd",
                CustomerId = customer.Id,
            });

            return View();
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