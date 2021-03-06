using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace PhotosOfUs.Web.Controllers
{
    
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("Landing");
        }

        public IActionResult Homepage()
        {
            return View("Landing");
        }

        public IActionResult Pricing()
        {
            return View();
        }

        public IActionResult Landing()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        [EnableCors("LoginPolicy")]
        public ActionResult AzureSignIn()
        {
            return View();
        }

        [AllowAnonymous]
        [EnableCors("LoginPolicy")]
        public ActionResult AzureSignUp()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
