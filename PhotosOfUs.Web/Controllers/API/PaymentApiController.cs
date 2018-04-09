using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;
using Stripe;
using System.Diagnostics;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Payment")]
    public class PaymentApiController : Controller
    {
        private PhotosOfUsContext _context;

        public PaymentApiController(PhotosOfUsContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Charge")]
        public StripeCharge Charge([FromBody] PaymentModel payment)
        {
            StripeConfiguration.SetApiKey("");
            Debug.WriteLine(payment);
            //int amount = payment.Amount;
            int amount = 500; // TODO: swap this out for the line above to have actual amounts sent
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

            return charge;
        }
    }
}
