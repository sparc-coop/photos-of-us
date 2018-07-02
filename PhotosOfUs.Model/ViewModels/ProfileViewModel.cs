using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.ViewModels
{
    public class ProfileViewModel
    {
        public int PhotographerId { get; set; }
        public string Name { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string PhotographerTitle { get; set; }
        public string ProfileText { get; set; }
        public string FacebookLink { get; set; }
        public string TwitterLink { get; set; }
        public string InstagramLink { get; set; }
        public string DribblLink { get; set; }

        public List<PhotoViewModel> Photos { get; set; }

        public static ProfileViewModel ToViewModel(List<Photo> photos, User photographer)
        {
            ProfileViewModel viewModel = new ProfileViewModel();

            viewModel.PhotographerId = photographer.Id;
            viewModel.Name = photographer.DisplayName;
            viewModel.PhotographerTitle = photographer.JobPosition; //TODO create column to this field
            viewModel.ProfileText = photographer.Bio; //TODO create column to this field
            viewModel.FacebookLink = photographer.Facebook;
            viewModel.TwitterLink = "twitter";//TODO create column to this field
            viewModel.InstagramLink = "instagram";//TODO create column to this field
            viewModel.ProfilePhotoUrl = photographer.ProfilePhotoUrl;
            viewModel.Photos = new List<PhotoViewModel>();

            foreach (var item in photos)
            {
                viewModel.Photos.Add(PhotoViewModel.ToViewModel(item));
            }

            return viewModel;
        }
    }
}
