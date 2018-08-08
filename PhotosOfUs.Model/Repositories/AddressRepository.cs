using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PhotosOfUs.Model.Repositories
{
    public class AddressRepository
    {
        private PhotosOfUsContext _context;

        public AddressRepository(PhotosOfUsContext context)
        {
            _context = context;
        }

        public Address Create(Address address)
        {
            var newAddress = new Address()
            {
                UserId = 1,
                FullName = address.FullName,
                Address1 = address.Address1,
                Address2 = address.Address2,
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode,
                Phone = address.Phone,
                Email = address.Email
            };

            try
            {
                _context.Address.Attach(newAddress);
                _context.SaveChanges();
                return address;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
            
            
        }

        public Address FindAddress(int userId)
        {
            return _context.Address.Where(x => x.UserId == userId).FirstOrDefault();
        }
    }
}
