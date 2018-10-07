using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kuvio.Kernel.Architecture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Repositories;
using PhotosOfUs.Model.ViewModels;
using Kuvio.Kernel.Auth;

namespace PhotosOfUs.Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Card")]
    public class CardApiController : Controller
    {
        private PhotosOfUsContext _context;
        private readonly IRepository<Card> _card;
        private readonly IRepository<User> _user;

        public CardApiController(PhotosOfUsContext context, IRepository<Card> cardRepository, IRepository<User> userRepository)
        {
            _context = context;
            _card = cardRepository;
            _user = userRepository;
        }

        [HttpGet]
        public List<CardViewModel> GetCard()
        {
            List<Card> pCards = _card.Where(x => x.PhotographerId == User.ID()).Include(x => x.Photographer).ToList();
            return pCards.Select(CardViewModel.ToViewModel).ToList();
        }

        [HttpPost]
        [Route("Create/{quantity}")]
        public List<CardViewModel> Create(int quantity)
        {
            var photographer = _user.Find(x => x.Id == User.ID());
            photographer.AddNewCards(quantity);
            _context.SaveChanges();

            return photographer.Card.Select(CardViewModel.ToViewModel).ToList();
        }
    }
}