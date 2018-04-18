using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;

namespace PhotosOfUs.Model.ViewModels
{
    public class SalesHistoryViewModel
    {
        public string UserDisplayName { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalMade { get; set; }
        public decimal TotalEarned { get; set; }
    
        public List<OrderViewModel> Orders { get; set; }

        public static SalesHistoryViewModel ToViewModel(User user, List<Order> orders)
        {
            var viewModel = new SalesHistoryViewModel();

            if (orders.Count > 0)
            {
                viewModel.UserDisplayName = orders.First().User.DisplayName;
            }

            viewModel.Orders = new List<OrderViewModel>();

            foreach (var order in orders)
            {
                viewModel.Orders.Add(OrderViewModel.ToViewModel(order));
            }

            viewModel.TotalSales = 0;
            viewModel.TotalMade = 0;
            viewModel.TotalEarned = 0;

            foreach (var orderModel in viewModel.Orders)
            {
                viewModel.TotalSales += orderModel.Amount;
                viewModel.TotalMade += orderModel.TotalPaid;
                viewModel.TotalEarned += orderModel.Earning;
            }

            return viewModel;
        }
    }
}
