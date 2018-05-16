using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class PhotoViewModel
    {
        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public int FolderId { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhotographerName { get; set; }
        public DateTime UploadDate { get; set; }
        public decimal? Price { get; set; }
        public string Resolution { get; set; }
        public string FileSize { get; set; }
        public User Photographer { get; set; }
        public string WaterMarkUrl { get; set; }

        public static PhotoViewModel ToViewModel(Photo entity)
        {
            PhotoViewModel viewModel = new PhotoViewModel();

            viewModel.Id = entity.Id;
            viewModel.PhotographerId = entity.PhotographerId;
            if (entity.Photographer != null)
            {
                viewModel.Photographer = entity.Photographer;
                viewModel.PhotographerName = entity.Photographer.DisplayName;
            }
            
            viewModel.FolderId = entity.FolderId;
            viewModel.Url = entity.Url;
            viewModel.ThumbnailUrl = entity.Url.Replace("/photos/", "/thumbnails/");
            viewModel.WaterMarkUrl = entity.Url.Replace("/photos/", "/watermark/");
            viewModel.Code = entity.Code;
            viewModel.Name = entity.Name;
            viewModel.UploadDate = entity.UploadDate;
            viewModel.Price = entity.Price;

            return viewModel;
        }

        public static List<PhotoViewModel> ToViewModel(List<Photo> entities)
        {
            List<PhotoViewModel> viewModels = new List<PhotoViewModel>();

            foreach (var item in entities)
            {
                viewModels.Add(ToViewModel(item));
            }

            return viewModels;
        }
    }
}
