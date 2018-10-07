using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kuvio.Kernel.Auth;
using Kuvio.Kernel.Architecture;
using PhotosOfUs.Web.Models;

namespace PhotosOfUs.Web.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private PhotosOfUsContext _context;
        private IRepository<Order> _order;

        public CustomerController(PhotosOfUsContext context, IRepository<Order>  orderRepository)
        {
            _context = context;
            _order = orderRepository;
        }

        public ActionResult Index()
        {
            Order order = _order.Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").FirstOrDefault();
            return View(CustomerOrderViewModel.ToViewModel(order));
        }

        public ActionResult OrderHistory(int id)
        {
            List<Order> orders = _order.Where(x => x.UserId == User.ID()).ToList();
            return View(CustomerOrderViewModel.ToViewModel(orders));
        }

        public ActionResult Confirmation()
        {
            List<Order> orders = _order.Where(x => x.UserId == User.ID()).ToList();
            return View(CustomerOrderViewModel.ToViewModel(orders));
        }
    }
}
