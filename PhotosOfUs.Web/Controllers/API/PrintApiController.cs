using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Web.Controllers.API
{
    [Route("api/Print")]
    public class PrintApiController : Controller
    {
        //pwinty sandbox
        //https://sandbox.pwinty.com/v2.6

        //[HttpPost]
        //[Route("OrderPhotos")]
        //public OrderViewModel CreateOrder(Order order)
        //{
        //    string url = "https://sandbox.pwinty.com/v2.5";

        //    var newOrder = new OrderViewModel();
        //    return newOrder;
        //}

    }
}
