﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.ViewModels;

namespace PhotosOfUs.Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Card")]
    public class CardApiController : Controller
    {
        private PhotosOfUsContext _context;

        public CardApiController(PhotosOfUsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<CardViewModel> GetCard()
        {
            var azureId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var photographerId = _context.UserIdentity.Find(azureId).UserID;
            List<Card> pCards = _context.Card.Where(x => x.PhotographerId == photographerId).Include(x=>x.Photographer).ToList();
            return pCards.Select(x=>CardViewModel.ToViewModel(x)).ToList();
        }
    }
}