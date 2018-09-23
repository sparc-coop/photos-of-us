using Kuvio.Kernel.Architecture;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.Models
{
    public class UserProfileUpdateCommandModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string JobPosition { get; set; }
        public string Bio { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Dribbble { get; set; }
        public bool? IsDeactivated { get; set; }
    }

    public class UserProfileUpdateCommand : Command<User>
    {
        public UserProfileUpdateCommand(IRepository<User> repository) : base(repository)
        {
        }

        public void Execute(UserProfileUpdateCommandModel model)
        {
            var user = Set.Find(model.Id);
            user.UpdateProfile(model.Email, model.FirstName, model.LastName, model.DisplayName, model.JobPosition, model.ProfilePhotoUrl, model.Bio);
            user.UpdateSocialMedia(model.Facebook, model.Instagram, model.Dribbble, model.Twitter);
            Commit();
        }
    }
}
