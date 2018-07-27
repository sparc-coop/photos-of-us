using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace PhotosOfUs.Model.Repositories
{
    public class OrderRepository
    {
        private PhotosOfUsContext _context;

        public OrderRepository(PhotosOfUsContext context)
        {
            _context = context;
        }

        public Order CreateOrder(int id)
        {
            Order order = new Order();
            order.OrderStatus = "Open";
            order.UserId = id;
            order.OrderDate = DateTime.Now;

            _context.Order.Add(order);
            _context.SaveChanges();
            return order;
        }

        public Order GetOrder(int orderId)
        {
            Order order = _context.Order.Where(x => x.Id == orderId).Include("OrderDetail").FirstOrDefault();
            return order;
        }

        public Order GetOpenOrder(int id)
        {
            Order order = _context.Order.Where(x => x.UserId == id && x.OrderStatus == "Open").Include("OrderDetail").FirstOrDefault();
            return order;
        }

        public OrderDetail CreateOrderDetails(int orderId, int photoId, int photographerId, int itemId, int quantity)
        {
            OrderDetail orderItem = new OrderDetail();
            Photo photo = _context.Photo.Where(x => x.Id == photoId).FirstOrDefault();
            PrintType type = _context.PrintType.Where(x => x.Id == itemId).FirstOrDefault();

            orderItem.OrderId = orderId;
            orderItem.PhotoId = photoId;
            orderItem.PrintTypeId = itemId;
            orderItem.Quantity = quantity;
            orderItem.UnitPrice = (decimal)photo.Price + (decimal)type.BaseCost;

            _context.OrderDetail.Add(orderItem);
            _context.SaveChanges();
            return orderItem;
        }

        public decimal GetOrderTotal(int id)
        {
            List<OrderDetail> orderDetails = _context.OrderDetail.Where(x => x.OrderId == id).ToList();
            decimal total = 0;
            foreach (var item in orderDetails)
            {
                total += (item.UnitPrice * item.Quantity);
            }

            return total;
        }

        public List<Order> GetUserOrders(int userId)
        {
            List<Order> orders = _context.Order.Where(x => x.UserId == userId).ToList();
            return orders;
        }

        public List<OrderDetail> GetOrderDetails(int orderId)
        {
            List<OrderDetail> orderItems = _context.OrderDetail.Where(x => x.OrderId == orderId).Include("Photo").Include("PrintType").ToList();
            return orderItems;
        }

        public List<OrderDetail> GetPhotographerOrderDetails(int photographerId)
        {
            List<OrderDetail> orderItems = _context.OrderDetail.Include("Photo").Where(x => x.Photo.PhotographerId == photographerId).ToList();

            return orderItems;
        }

        public List<Order> OrderHistory(int userId)
        {
            return _context.Order.Where(x => x.UserId == userId).ToList();
        }

        //public List<Order> GetPhotographerOrders(List<OrderDetail> orderItems)
        //{
        //    List<Order> orders = new List<Order>();
        //    var orderIds = orderItems.GroupBy(x => x.OrderId);
                
        //    return orders;
        //}


        public List<Order> GetOrders(int userId, SalesQueryModel query = null)
        {
            var final = _context.Order
               .Include(order => order.OrderDetail)
                   .ThenInclude(orderDetail => orderDetail.PrintType)
               .Include(order => order.OrderDetail)
                   .ThenInclude(orderDetail => orderDetail.Photo)
               .Include(x => x.User)
               //.Where(x => x.OrderDetail.Select(y => y.Photo).First().PhotographerId == userId);
               .Where(x => x.UserId == userId);

            //if (query.Total != null) {
            //    final = final.Where(x => x.Total >= int.Parse(query.Total));
            //}
            if (query.PhotoName != null)
            {
                Regex regex = new Regex(query.PhotoName, RegexOptions.IgnoreCase);
                final = final.Where(x => regex.IsMatch(x.OrderDetail.Select(y => y.Photo).First().Name));
            }
            if (query.OrderStatus != null)
            {
                final = final.Where(x => x.OrderStatus == query.OrderStatus);
            }
            if (query.OrderDateEarliest != null && query.OrderDateLatest != null)
            {
                final = final.Where(x => x.OrderDate.CompareTo(query.OrderDateEarliest) >= 0 && x.OrderDate.CompareTo(query.OrderDateLatest) <= 0);
            }
            if (query.Email != null)
            {
                Regex regex = new Regex(query.Email, RegexOptions.IgnoreCase);
                final = final.Where(x => regex.IsMatch(x.User.Email));
            }
            if (query.FirstName != null)
            {
                Regex regex = new Regex(query.FirstName, RegexOptions.IgnoreCase);
                final = final.Where(x => regex.IsMatch(x.User.FirstName));
            }
            if (query.LastName != null)
            {
                Regex regex = new Regex(query.LastName, RegexOptions.IgnoreCase);
                final = final.Where(x => regex.IsMatch(x.User.LastName));
            }
            if (query.DisplayName != null)
            {
                Regex regex = new Regex(query.DisplayName, RegexOptions.IgnoreCase);
                final = final.Where(x => regex.IsMatch(x.User.DisplayName));
            }
            if (query.IsPhotographer != null)
            {
                final = final.All(x => x.User.IsPhotographer != null)
                                ? final.Where(x => x.User.IsPhotographer == query.IsPhotographer)
                                : final.Where(x => query.IsPhotographer == false);
            }
            if (query.QuantityMin != null && query.QuantityMax != null)
            {
                final = final.Where(x => x.OrderDetail.Select(y => y.Quantity).First() >= int.Parse(query.QuantityMin) && x.OrderDetail.Select(y => y.Quantity).First() <= int.Parse(query.QuantityMax));
            }

            if (final == null)
            {
                Debug.WriteLine("NO FINAL---------------------------------");
                return new List<Order>();
            }

            Debug.WriteLine("IS FINAL: {0}---------------------------------", final.Count());
            return final.ToList();
        }

        //public List<Order> SearchOrders(int userId, string query)
        //{
        //    var loweredQuery = query.ToLower();

        //    var result = _context.Order.AsQueryable();

        //    result = result.Where(x => x.UserId == userId);

        //    result = result.Where(order => order.OrderStatus.ToLower().Contains(loweredQuery)
        //                                   || order.OrderDetail.First().Photo.Name.ToLower().Contains(loweredQuery)
        //                                   || order.OrderDetail.First().PrintType.Type.ToLower().Contains(loweredQuery));

        //    result = result
        //        .Include(order => order.OrderDetail)
        //            .ThenInclude(orderDetail => orderDetail.PrintType)
        //        .Include(order => order.OrderDetail)
        //            .ThenInclude(orderDetail => orderDetail.Photo)
        //        .Include(x => x.User);

        //    return result.ToList();
        //}
    }
}