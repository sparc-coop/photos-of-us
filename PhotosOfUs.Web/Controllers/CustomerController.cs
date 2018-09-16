﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhotosOfUs.Web.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private PhotosOfUsContext _context;
        private OrderRepository _orderRepository;

        public CustomerController(PhotosOfUsContext context, OrderRepository orderRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
        }

        public ActionResult Index()
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;
            var user = _context.User.Find(photographerId);

            return View(UserViewModel.ToViewModel(user));
        }

        public ActionResult OrderHistory(int id)
        {
            List<Order> orders = _orderRepository.OrderHistory(id);
            return View(CustomerOrderViewModel.ToViewModel(orders));
        }

        public ActionResult Confirmation()
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _context.UserIdentity.Find(azureId).UserID;
            List<Order> orders = _orderRepository.OrderHistory(userId);
            return View(CustomerOrderViewModel.ToViewModel(orders));
        }
    }
}
