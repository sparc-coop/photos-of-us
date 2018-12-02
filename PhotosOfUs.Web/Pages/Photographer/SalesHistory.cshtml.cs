using Kuvio.Kernel.Architecture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotosOfUs.Model.Models;
using Kuvio.Kernel.Auth;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace PhotosOfUs.Pages.Photographer
{
    [Authorize]
    public class SalesHistoryModel : PageModel
    {
        private IRepository<Order> _orders;

        public List<Model> SalesHistory { get; set; }
        public List<Order> Orders { get; set; }
        public string TotalSales { get; set; }
        public string TotalMade { get; set; }
        public string TotalEarned { get; set; }

        public SalesHistoryModel(IRepository<Order> orders)
        {
            _orders = orders;
        }

        public void OnGet()
        {
            var orderLines = _orders.Where(x => x.OrderDetail.Any(y => y.Photo.PhotographerId == User.ID()))
                .SelectMany(x => x.OrderDetail.Where(y => y.Photo.PhotographerId == User.ID()));

            SalesHistory = orderLines.GroupBy(x => new { x.PhotoId, x.Photo.Name })
                .Select(x => new Model
                {
                    PhotoId = x.Key.PhotoId,
                    PhotoName = x.Key.Name,
                    Quantity = x.Sum(y => y.Quantity),
                    UnitPrice = x.Sum(y => y.UnitPrice),
                    Earnings = x.Sum(y => y.Photo.Price)
                }).ToList();

            TotalSales = $"{SalesHistory.Sum(x => x.Quantity):N0}";
            TotalMade = $"{SalesHistory.Sum(x => x.Total):C}";
            TotalEarned = $"{SalesHistory.Sum(x => x.Earnings):C}";
        }

        public class Model {
            public int PhotoId;
            public string PhotoName;
            public int Quantity;
            public decimal? UnitPrice;
            public decimal? Total => Quantity * UnitPrice;
            public decimal? Earnings;
        }
    }
}