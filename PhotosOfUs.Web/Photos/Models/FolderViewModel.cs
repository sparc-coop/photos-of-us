using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class FolderViewModel
    {
        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<PhotoViewModel> Photo { get; set; }

        //public static FolderViewModel ToViewModel(Folder entity)
        //{
        //    FolderViewModel viewModel = new FolderViewModel();

        //    viewModel.Id = entity.Id;
        //    viewModel.PhotographerId = entity.PhotographerId;
        //    viewModel.Name = entity.Name;
        //    viewModel.CreatedDate = entity.CreatedDate;
        //    viewModel.Photos = new List<PhotoViewModel>();

        //    foreach (var item in entity.Photo)
        //    {
        //        viewModel.Photos.Add(PhotoViewModel.ToViewModel(item));
        //    }

        //    return viewModel;
        //}

        //public static List<FolderViewModel> ToViewModel(List<Folder> entities)
        //{
        //    List<FolderViewModel> viewModels = new List<FolderViewModel>();

        //    foreach (var item in entities)
        //    {
        //        viewModels.Add(ToViewModel(item));
        //    }

        //    return viewModels;
        //}
    }

    public class FolderRenameViewModel
    {
        public int Id { get; set; }
        public string NewName { get; set; }
    }
}
