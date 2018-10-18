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
        
        //public List<TagViewModel> Tags { get; set; }

        public static TagViewModel ToViewModel(Tag entity)
        {
            TagViewModel viewModel = new TagViewModel();

            viewModel.Id = entity.Id;
            viewModel.Name = entity.Name;
            viewModel.text = entity.Name;

            return viewModel;
        }

        public static TagViewModel ToViewModel(PhotoTag entity)
        {
            TagViewModel viewModel = new TagViewModel();

            viewModel.Id = entity.Tag.Id;
            viewModel.Name = entity.Tag.Name;

            return viewModel;
        }

        public static Tag ToEntity(TagViewModel viewModel)
        {
            Tag entity = new Tag();

            entity.Id = viewModel.Id;
            entity.Name = viewModel.Name;

            return entity;
        }

        public static List<Tag> ToEntity(List<TagViewModel> viewModels)
        {
            List<Tag> entity = new List<Tag>();

            foreach (var item in viewModels)
            {
                entity.Add(ToEntity(item));
            }

            return entity;
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

        public static List<TagViewModel> ToViewModel(List<PhotoTag> entities)
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
