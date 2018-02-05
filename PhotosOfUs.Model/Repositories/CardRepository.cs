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
            newCard.Code = CodeHelper.NewCode(7); //check if code exist
            newCard.CreatedDate = DateTime.Today;

            _context.Card.Add(newCard);
            _context.SaveChanges();

            return newCard;
        }
    }
}
