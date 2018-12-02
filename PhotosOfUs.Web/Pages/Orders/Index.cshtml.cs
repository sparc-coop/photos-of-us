using Kuvio.Kernel.Architecture;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Model.Models;
using System.Linq;

namespace PhotosOfUs.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private IRepository<Order> _orders;
        public string UserName { get; set; }
        public int? OrderId { get; set; }

        public IndexModel(IRepository<Order> orders)
        {
            _orders = orders;
        }

        public void OnGet()
        {
            OrderId = _orders.Where(x => x.UserId == User.ID() && x.OrderStatus == "Open").Select(x => x.Id).FirstOrDefault();
            UserName = User.DisplayName();
        }
    }
}