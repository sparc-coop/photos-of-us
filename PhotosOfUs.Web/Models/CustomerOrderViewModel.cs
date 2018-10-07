using System;
using System.Collections.Generic;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Model.ViewModels
{
    public class CustomerOrderViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName {get;set;}
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal? Total { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }

        public static CustomerOrderViewModel ToViewModel(Order order)
        {
            CustomerOrderViewModel viewModel = new CustomerOrderViewModel();

            viewModel.Id = order.Id;
            viewModel.UserId = order.UserId;
            viewModel.UserName = order.User.DisplayName;
            viewModel.OrderStatus = order.OrderStatus;
            viewModel.OrderDate = order.OrderDate;
            viewModel.Total = order.Total;

            return viewModel;
        }

        public static List<CustomerOrderViewModel> ToViewModel(List<Order> entities)
        {
            List<CustomerOrderViewModel> viewModels = new List<CustomerOrderViewModel>();

            foreach (var item in entities)
            {
                viewModels.Add(ToViewModel(item));
            }

            return viewModels;
        }
    }
}
