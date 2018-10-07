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
using Kuvio.Kernel.Auth;
using Kuvio.Kernel.Architecture;

namespace PhotosOfUs.Web.Controllers
{
    [Authorize]
    public class PhotoController : Controller
    {
        private PhotosOfUsContext _context;
        private IRepository<Order> _order;
        private IRepository<Photo> _photo;

        public PhotoController(PhotosOfUsContext context, IRepository<Order> orderRepository, IRepository<Photo> photoRepository)
        {
            _context = context;
            _order = orderRepository;
            _photo = photoRepository;
        }

        public ActionResult Purchase(int id)
        {
            var photo = _photo.Find(x => x.Id == id);
            var viewModel = PhotoViewModel.ToViewModel(photo);

            return View(viewModel);
        }

        public ActionResult Cart(int id)
        {
            Order order = _order.Where(x => x.UserId == id && x.OrderStatus == "Open").FirstOrDefault();
            return View(CustomerOrderViewModel.ToViewModel(order));
        }

        public ActionResult Checkout(int id)
        {
            Order order = _order.Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").FirstOrDefault();
            return View(CustomerOrderViewModel.ToViewModel(order));
        }

        [HttpPost]
        public IActionResult Charge(string stripeToken)
        {
            StripeConfiguration.SetApiKey("");

            Order userOrder = _order.Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").FirstOrDefault();
            userOrder.SetStatusToPending();
            _order.Commit();
            
            Order order = _order.Find(x => x.Id == userOrder.Id);
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