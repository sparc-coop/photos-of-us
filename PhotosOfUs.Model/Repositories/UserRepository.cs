using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotosOfUs.Model.ViewModels;

namespace PhotosOfUs.Model.Repositories
{
    public class UserRepository
    {
        private PhotosOfUsContext _context;

        public UserRepository(PhotosOfUsContext context)
        {
            _context = context;
        }

        public User Find(int userId)
        {
            return _context.User.Find(userId);
        }

        public bool UpdateAccountProfileSettings(ProfileSettingsViewModel model)
        {
            var user = Find(model.UserId);

            if(null != model.Email)
                user.Email = model.Email;

            user.LastName = model.LastName;
            user.ProfilePhotoUrl = model.ProfilePhotoUrl;
            user.FirstName = model.FirstName;
            user.JobPosition = model.JobPosition;
            user.Bio = model.Bio;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool UpdateAccountSettings(PhotographerAccountViewModel model)
        {
            var user = Find(model.Id);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DisplayName = model.DisplayName;
            user.JobPosition = model.JobPosition;
            user.Bio = model.Bio;

            _context.SaveChanges();

            return true;
        }

        public Address GetAddress(int userId)
        {
            return _context.Address.Where(x => x.UserId == userId).First();
        }
    }
}
