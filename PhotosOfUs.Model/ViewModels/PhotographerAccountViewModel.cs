using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class PhotographerAccountViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string JobPosition { get; set; }
        public string Bio { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public bool? IsPhotographer { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Dribbble { get; set; }
        public bool? IsDeactivated { get; set; }

        public static PhotographerAccountViewModel ToViewModel(User u)
        {
            PhotographerAccountViewModel model = new PhotographerAccountViewModel();
            model.Id = u.Id;
            model.Email = u.Email;
            model.FirstName = u.FirstName;
            model.LastName = u.LastName;
            model.DisplayName = u.DisplayName;
            model.JobPosition = u.JobPosition;
            model.Bio = u.Bio;
            model.ProfilePhotoUrl = u.ProfilePhotoUrl;
            model.IsPhotographer = u.IsPhotographer;
            model.Facebook = u.Facebook;
            model.Twitter = u.Twitter;
            model.Instagram = u.Instagram;
            model.Dribbble = u.Dribbble;
            model.IsDeactivated = u.IsDeactivated;
            
            return model;
        }
    }

   
}
