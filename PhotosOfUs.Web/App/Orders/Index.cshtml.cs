using Kuvio.Kernel.Architecture;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Model.Models;
using System.Linq;
using System.Collections.Generic;

namespace PhotosOfUs.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private IRepository<Order> _orders;
        public List<Order> Orders { get; private set; }

        public IndexModel(IRepository<Order> orders)
        {
            _orders = orders;
        }

        public void OnGet()
        {
            Orders = _orders.Include("OrderDetail.Photo").Where(x => x.UserId == User.ID()).ToList();
        }
    }
}