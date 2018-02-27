using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return _context.User.Find(userId);//.Where(x => x.Id == userId).First();
        }

        public Address GetAddress(int userId)
        {
            return _context.Address.Where(x => x.UserId == userId).First();
        }
    }
}
