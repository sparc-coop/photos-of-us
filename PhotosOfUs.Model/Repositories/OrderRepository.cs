using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PhotosOfUs.Model.ViewModels;
using System.Text.RegularExpressions;

namespace PhotosOfUs.Model.Repositories
{
    public class OrderRepository
    {
        private PhotosOfUsContext _context;

        public OrderRepository(PhotosOfUsContext context)
        {
            _context = context;
        }

        public List<Order> GetOrders(int userId, SalesQueryModel query = null)
        {
            var final = _context.Order
               .Include(order => order.OrderDetail)
                   .ThenInclude(orderDetail => orderDetail.PrintType)
               .Include(order => order.OrderDetail)
                   .ThenInclude(orderDetail => orderDetail.Photo)
               .Include(x => x.User)
               .Where(x => x.OrderDetail.Select(y => y.Photo).First().PhotographerId == userId);

            //if (query.Total != null) {
            //    final = final.Where(x => x.Total >= int.Parse(query.Total));
            //}
            if (query.OrderStatus != null) {
                final = final.Where(x => x.OrderStatus == query.OrderStatus);
            }
            if (query.OrderDate != null) {
                final = final.Where(x => x.OrderDate.CompareTo(query.OrderDate) >= 0);
            }
            if (query.Email != null) {
                Regex regex = new Regex(query.Email, RegexOptions.IgnoreCase);
                final = final.Where(x => regex.IsMatch(x.User.Email));
            }
            if (query.FirstName != null) {
                Regex regex = new Regex(query.FirstName, RegexOptions.IgnoreCase);
                final = final.Where(x => regex.IsMatch(x.User.FirstName));
            }
            if (query.LastName != null) {
                Regex regex = new Regex(query.LastName, RegexOptions.IgnoreCase);
                final = final.Where(x => regex.IsMatch(x.User.LastName));
            }
            if (query.DisplayName != null) {
                Regex regex = new Regex(query.DisplayName, RegexOptions.IgnoreCase);
                final = final.Where(x => regex.IsMatch(x.User.DisplayName));
            }
            if (query.IsPhotographer != null) {
                final = final.All(x => x.User.IsPhotographer != null)
                                ? final.Where(x => x.User.IsPhotographer == query.IsPhotographer)
                                : final.Where(x => query.IsPhotographer == false);
            }
            if (query.Quantity != null) {
                final = final.Where(x => x.OrderDetail.Select(y => y.Quantity).First() >= int.Parse(query.Quantity));
            }

            if(final == null)
            {
                return new List<Order>();
            }

            return final.ToList();
        }
    }
}