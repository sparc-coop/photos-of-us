using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class TagViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string text { get; set; }
        
        public static TagViewModel ToViewModel(Tag entity)
        {
            TagViewModel viewModel = new TagViewModel();

            viewModel.Id = entity.Id;
            viewModel.Name = entity.Name;
            viewModel.text = entity.Name;

            return viewModel;
        }

        public static List<TagViewModel> ToViewModel(List<Tag> entities)
        {
            List<TagViewModel> viewModels = new List<TagViewModel>();

            foreach (var item in entities)
            {
                viewModels.Add(ToViewModel(item));
            }

            return viewModels;
        }
    }
}
