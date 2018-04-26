using System;
using System.Collections.Generic;
using System.Text;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Model.ViewModels
{
    public class ProfileSettingsViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string JobPosition { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }

        public static ProfileSettingsViewModel ToViewModel(User entity)
        {
            var model = new ProfileSettingsViewModel();
            model.UserId = entity.Id;
            model.FirstName = entity.FirstName;
            model.LastName = entity.LastName;
            model.ProfilePhotoUrl = entity.ProfilePhotoUrl;
            model.JobPosition = entity.JobPosition;
            model.Email = entity.Email;
            model.Bio = entity.Bio;

            return model;
        }
    }
}
