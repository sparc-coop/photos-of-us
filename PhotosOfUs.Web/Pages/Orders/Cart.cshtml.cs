using Kuvio.Kernel.Architecture;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Model.Models;
using System.Linq;
using System.Collections.Generic;

namespace PhotosOfUs.Pages.Orders
{
    public class CartModel : PageModel
    {
        private IRepository<Order> _orders;
        public int OrderId { get; set; }
        public int UserId { get; set; }

        public CartModel(IRepository<Order> orders)
        {
            _orders = orders;
        }

        public void OnGet(int id)
        {
            OrderId = id;
            UserId = User.ID();
        }
    }
}