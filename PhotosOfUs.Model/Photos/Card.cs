using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotosOfUs.Model.Models
{
    public partial class Card
    {
        public int Id { get; set; }
        public int PhotographerId { get; set; }        
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public User Photographer { get; set; }

        // Not Mapped
        public string Url => "www.photosof.us/" + Photographer.DisplayName.ToLower().Replace(" ","");

        public Card() {}

        public Card(User user)
        {
            PhotographerId = user.Id;
            CreatedDate = DateTime.Today;
            do
            {
                Code = NewCode(7);
            } while (!user.Card.Any(x => x.Code == Code));
        }

        private static Random random = new Random();
        private string NewCode(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
