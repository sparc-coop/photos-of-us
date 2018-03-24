using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PhotosOfUs.Model.Repositories
{
    public class OrderRepository
    {
        private PhotosOfUsContext _context;

        public OrderRepository(PhotosOfUsContext context)
        {
            _context = context;
        }

        public List<Order> GetOrders(int userId)
        {
            return _context.Order
                .Include(order => order.OrderDetail)
                    .ThenInclude(orderDetail => orderDetail.PrintType)
                .Include(order => order.OrderDetail)
                    .ThenInclude(orderDetail => orderDetail.Photo)
                .Include(x => x.User)
                .Where(x => x.UserId == userId).ToList();
        }
    }
}
