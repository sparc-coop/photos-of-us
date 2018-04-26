using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class PrintTypeViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Height { get; set; }
        public string Length { get; set; }
        public string Icon { get; set; }

        public static PrintTypeViewModel ToViewModel(PrintType entity)
        {
            PrintTypeViewModel viewModel = new PrintTypeViewModel();

            viewModel.Id = entity.Id;
            viewModel.Type = entity.Type;
            viewModel.Height = entity.Height;
            viewModel.Length = entity.Length;
            viewModel.Icon = entity.Icon;


            return viewModel;
        }

        public static List<PrintTypeViewModel> ToViewModel(List<PrintType> entities)
        {
            List<PrintTypeViewModel> viewModels = new List<PrintTypeViewModel>();

            foreach (var item in entities)
            {
                viewModels.Add(ToViewModel(item));
            }

            return viewModels;
        }
    }
}
