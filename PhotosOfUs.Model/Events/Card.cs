using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotosOfUs.Model.Models
{
    public partial class Card
    {
        public int Id { get; set; }
        public int EventId { get; set; }        
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<Photo> Photos { get; set; }

        // Not Mapped
        //public string Url => "www.photosof.us/" + Photographer.DisplayName.ToLower().Replace(" ","");

        public Card() {}

        public Card(Event ev)
        {
            EventId = ev.EventId;
            CreatedDate = DateTime.Now;
            do
            {
                Code = NewCode(7);
            } while (ev.Cards.Any(x => x.Code == Code));
        }

        private static readonly Random random = new Random();
        private string NewCode(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
