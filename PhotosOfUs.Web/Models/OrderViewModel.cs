using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Model.ViewModels
{
    public class OrderViewModel
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string PhotoName { get; set; }
        public string PrintSize { get; set; }
        public string OrderStatus { get; set; }
        public int Amount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Earning { get; set; }
        public DateTime OrderDate { get; set; }
        
        public static OrderViewModel ToViewModel(Models.Order order)
        {
            var viewModel = new OrderViewModel();

            viewModel.Id = order.Id;
            viewModel.UserId = order.UserId;
            viewModel.OrderStatus = order.OrderStatus;
            viewModel.Amount = order.OrderDetail.First().Quantity;
            viewModel.TotalPaid = order.OrderDetail.First().UnitPrice * viewModel.Amount;
            viewModel.Earning = viewModel.TotalPaid * (decimal) 0.955;
            viewModel.OrderDate = order.OrderDate;

            return viewModel;
        }

        public static List<OrderViewModel> ToViewModel(List<Models.Order> entities)
        {
            List<OrderViewModel> viewModels = new List<OrderViewModel>();

            foreach (var item in entities)
            {
                viewModels.Add(ToViewModel(item));
            }

            return viewModels;
        }
    }
}
