using Kuvio.Kernel.Architecture;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Model.Models;
using System.Linq;
using System.Collections.Generic;

namespace PhotosOfUs.Pages.Orders
{
    public class ConfirmationModel : PageModel
    {
        private readonly IRepository<User> _users;
        private readonly IRepository<Order> _orders;

        public User CurrentUser { get; set; }
        public int TotalItems { get; set; }
        public decimal? TotalAmount { get; set; }

        public ConfirmationModel(IRepository<User> users, IRepository<Order> orders)
        {
            _users = users;
            _orders = orders;
        }

        public void OnGet(int id)
        {
            CurrentUser = _users.Find(x => x.Id == User.ID());
            var order = _orders.Include(x => x.OrderDetail).Find(x => x.UserId == User.ID() && x.OrderStatus == "Open");
            TotalItems = order.OrderDetail.Count();
            TotalAmount = order.CalculatedTotal;
        }
    }
}