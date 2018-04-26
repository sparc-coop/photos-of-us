using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string JobPosition { get; set; }
        public string Bio { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool? IsPhotographer { get; set; }

        public ICollection<Card> Card { get; set; }
        public ICollection<Folder> Folder { get; set; }
        public ICollection<Order> Order { get; set; }
        public ICollection<Photo> Photo { get; set; }
        public ICollection<PrintPrice> PrintPrice { get; set; }
        public Address Address { get; set; }

        public static UserViewModel ToViewModel(User entity)
        {
            UserViewModel viewModel = new UserViewModel();

            viewModel.Id = entity.Id;
            viewModel.Email = entity.Email;
            viewModel.FirstName = entity.FirstName;
            viewModel.LastName = entity.LastName;
            viewModel.DisplayName = entity.DisplayName;
            viewModel.CreateDate = entity.CreateDate;
            viewModel.IsPhotographer = entity.IsPhotographer;
            viewModel.JobPosition = entity.JobPosition;
            viewModel.Bio = entity.Bio;
            viewModel.ProfilePhotoUrl = entity.ProfilePhotoUrl;

            return viewModel;
        }
    }
}
