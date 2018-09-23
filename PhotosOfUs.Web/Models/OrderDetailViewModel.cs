using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int PhotoId { get; set; }
        public int Quantity { get; set; }
        public int? PrintTypeId { get; set; }
        public decimal UnitPrice { get; set; }

        //public Order Order { get; set; }
        public Photo Photo { get; set; }
        public PrintType PrintType { get; set; }

        public static OrderDetailViewModel ToViewModel(OrderDetail orderDetail)
        {
            OrderDetailViewModel viewModel = new OrderDetailViewModel();

            viewModel.Id = orderDetail.Id;
            viewModel.OrderId = orderDetail.OrderId;
            viewModel.PhotoId = orderDetail.PhotoId;
            viewModel.Quantity = orderDetail.Quantity;
            viewModel.PrintTypeId = orderDetail.PrintTypeId;
            viewModel.UnitPrice = orderDetail.UnitPrice;
            viewModel.Photo = orderDetail.Photo;
            viewModel.PrintType = orderDetail.PrintType;

            return viewModel;
        }

        public static List<OrderDetailViewModel> ToViewModel(List<OrderDetail> entities)
        {
            List<OrderDetailViewModel> viewModels = new List<OrderDetailViewModel>();

            foreach (var item in entities)
            {
                viewModels.Add(ToViewModel(item));
            }

            return viewModels;
        }
    }
}
