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

        public List<Order> SearchOrders(int userId, string query)
        {
            var loweredQuery = query.ToLower();

            var result = _context.Order.AsQueryable();

            result = result.Where(x => x.UserId == userId);

            result = result.Where(order => order.OrderStatus.ToLower().Contains(loweredQuery)
                                           || order.OrderDetail.First().Photo.Name.ToLower().Contains(loweredQuery)
                                           || order.OrderDetail.First().PrintType.Type.ToLower().Contains(loweredQuery));

            result = result
                .Include(order => order.OrderDetail)
                    .ThenInclude(orderDetail => orderDetail.PrintType)
                .Include(order => order.OrderDetail)
                    .ThenInclude(orderDetail => orderDetail.Photo)
                .Include(x => x.User);

            return result.ToList();
        }
    }
}
