using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Model.ViewModels
{
    public class OrderViewModel
    {
        public string PhotoName { get; set; }
        public string PrintSize { get; set; }
        public string OrderStatus { get; set; }
        public int Amount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Earning { get; set; }
        
        public static OrderViewModel ToViewModel(Order order)
        {
            var viewModel = new OrderViewModel();

            viewModel.PhotoName = order.OrderDetail.First().Photo.Name;
            viewModel.PrintSize = order.OrderDetail.First().PrintType.Type;
            viewModel.OrderStatus = order.OrderStatus;
            viewModel.Amount = order.OrderDetail.First().Quantity;
            viewModel.TotalPaid = order.OrderDetail.First().UnitPrice * viewModel.Amount;
            viewModel.Earning = viewModel.TotalPaid * (decimal) 0.955;

            return viewModel;
        }
    }
}
