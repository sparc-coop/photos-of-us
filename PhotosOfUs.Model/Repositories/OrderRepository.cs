using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.Repositories
{
    class OrderRepository
    {
        private PhotosOfUsContext _context;

        public OrderRepository(PhotosOfUsContext context)
        {
            _context = context;
        }


    }
}
