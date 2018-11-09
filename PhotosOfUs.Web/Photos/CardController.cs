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
using PhotosOfUs.Web.Utilities;

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
            //List<Card> pCards = _card.Where(x => x.PhotographerId == User.ID()).Include(x => x.Photographer).ToList();
            User user = _user.Include(x => x.UserIdentities).Find(x => x.UserIdentities.Any(y => y.AzureID == User.AzureID()));
            List<Card> pCards = _card.Where(x => x.PhotographerId == user.Id).Include(x => x.Photographer).ToList();
            return pCards.ToList().ToViewModel<List<CardViewModel>>();
        }

        [HttpPost]
        [Route("Create/{quantity:int}/{userId:int}")]
        public List<Card> Create(int quantity, int userId)
        {
            var photographer = _user.Find(x => x.Id == userId);
            photographer.AddNewCards(quantity);
            _user.Commit();

            return photographer.Card.ToList();
        }
    }
}