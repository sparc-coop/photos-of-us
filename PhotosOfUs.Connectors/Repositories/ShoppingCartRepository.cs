/* using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotosOfUs.Model.Repositories
{
    public class ShoppingCartRepository
    {
        private PhotosOfUsContext _context;
        public const string CartSessionKey = "CartCode";
        private string ShoppingCartCode;

        public ShoppingCartRepository(PhotosOfUsContext context, HttpContext httpContext)
        {
            _context = context;
            ShoppingCartCode = GetCartCode(httpContext);
        }

        public ShoppingCartItem GetCart(HttpContext context)
        {
            var cart = new ShoppingCartItem();
            cart.CartCode = GetCartCode(context);

            return cart;
        }
        // Helper method to simplify shopping cart calls
        public ShoppingCartItem GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        public void AddToCart(Photo photo)
        {
            // Get the matching cart and photo instances
            var cartItem = _context.ShoppingCart.SingleOrDefault(
                c => c.CartCode == ShoppingCartCode
                && c.PhotoId == photo.Id);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new ShoppingCartItem
                {
                    PhotoId = photo.Id,
                    CartCode = ShoppingCartCode,
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };

                _context.ShoppingCart.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Quantity++;
            }
            // Save changes
            _context.SaveChanges();
        }
        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = _context.ShoppingCart.Single(
                cart => cart.CartCode == ShoppingCartCode
                && cart.PhotoId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    itemCount = cartItem.Quantity;
                }
                else
                {
                    _context.ShoppingCart.Remove(cartItem);
                }
                // Save changes
                _context.SaveChanges();
            }
            return itemCount;
        }
        public void EmptyCart()
        {
            var cartItems = _context.ShoppingCart.Where(
                cart => cart.CartCode == ShoppingCartCode);

            foreach (var cartItem in cartItems)
            {
                _context.ShoppingCart.Remove(cartItem);
            }
            // Save changes
            _context.SaveChanges();
        }
        public List<ShoppingCartItem> GetCartItems()
        {
            return _context.ShoppingCart.Where(
                cart => cart.CartCode == ShoppingCartCode).ToList();
        }
        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in _context.ShoppingCart
                          where cartItems.CartCode == ShoppingCartCode
                          select (int?)cartItems.Quantity).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }
        public decimal GetTotal()
        {
            // Multiply photo price by count of that photo to get 
            // the current price for each of those photos in the cart
            // sum all photo price totals to get the cart total
            decimal? total = (from cartItems in _context.ShoppingCart
                              where cartItems.CartCode == ShoppingCartCode
                              select (int?)cartItems.Quantity *
                              cartItems.Photo.Price).Sum();

            return total ?? decimal.Zero;
        }
        public Order CreateOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = GetCartItems();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    PhotoId = item.PhotoId,
                    OrderId = order.Id,
                    UnitPrice = item.Photo.Price.Value,
                    Quantity = item.Quantity
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Quantity * item.Photo.Price.Value);

                _context.OrderDetail.Add(orderDetail);

            }
            order.Total = orderTotal;

            _context.SaveChanges();
            EmptyCart();
            return order;
        }
        // We're using HttpContextBase to allow access to cookies.
        public string GetCartCode(HttpContext context)
        {
            if (context.Session.Get(CartSessionKey) == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session.SetString(CartSessionKey, context.User.Identity.Name);
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    context.Session.SetString(CartSessionKey, tempCartId.ToString());
                }
            }
            return context.Session.Get(CartSessionKey).ToString();
        }

        public void MigrateCart(ShoppingCartItem cart, int userId)
        {
            cart.UserId = userId;

            // store cart on the DB
        }
    }
}
 */