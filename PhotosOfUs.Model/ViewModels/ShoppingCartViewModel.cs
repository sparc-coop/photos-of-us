using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class ShoppingCartViewModel
    {
        public int Id { get; set; }
        public string CartCode { get; set; }
        public int UserId { get; set; }
        public int PhotoId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateCreated { get; set; }
        public virtual Photo Photo { get; set; }

        public static ShoppingCartViewModel ToViewModel(ShoppingCartItem entity)
        {
            ShoppingCartViewModel viewModel = new ShoppingCartViewModel();

            viewModel.Id = entity.Id;
            viewModel.CartCode = entity.CartCode;
            viewModel.UserId = entity.UserId;
            viewModel.PhotoId = entity.PhotoId;
            viewModel.Quantity = entity.Quantity;
            viewModel.DateCreated = entity.DateCreated;
            viewModel.Photo = entity.Photo;

            return viewModel;
        }

        public static List<ShoppingCartViewModel> ToViewModel(List<ShoppingCartItem> entities)
        {
            List<ShoppingCartViewModel> viewModels = new List<ShoppingCartViewModel>();

            foreach (var item in entities)
            {
                viewModels.Add(ToViewModel(item));
            }

            return viewModels;
        }
    }
}
