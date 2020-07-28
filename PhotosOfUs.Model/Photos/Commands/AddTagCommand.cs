using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Core.Photos.Commands
{
    public class TagViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }

        public static TagViewModel ToViewModel(Tag entity)
        {
            TagViewModel viewModel = new TagViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Text = entity.Name
            };

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

    public class AddTagCommand
    {
    }
}
