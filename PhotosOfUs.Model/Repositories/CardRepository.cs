using PhotosOfUs.Model.Models;
using PhotosOfUs.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotosOfUs.Model.Repositories
{
    public class CardRepository
    {
        private PhotosOfUsContext _context;

        public CardRepository(PhotosOfUsContext context)
        {
            _context = context;
        }

        public List<Card> GetCards(int photographerId)
        {
            return _context.Card.Where(c => c.PhotographerId == photographerId).ToList();
        }

        public Card Add(int photographerId)
        {
            Card newCard = new Card();
            newCard.PhotographerId = photographerId;
            newCard.Code = CodeHelper.NewCode(7); //check if code exists
            newCard.CreatedDate = DateTime.Today;

            _context.Card.Add(newCard);
            _context.SaveChanges();
            _context.Entry(newCard).Reference(x => x.Photographer).Load();

            return newCard;
        }

        public List<Card> AddMultiple(int photographerId, int quantity)
        {
            List<Card> newCards = new List<Card>();
            for (int i = 0; i < quantity; i++)
            {
                newCards.Add(Add(photographerId));
            }
            return newCards;
        }
    }
}
