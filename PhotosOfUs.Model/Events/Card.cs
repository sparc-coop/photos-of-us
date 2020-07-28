using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotosOfUs.Core.Events
{
    public partial class Card
    {
        public int Id { get; protected set; }
        public int EventId { get; protected set; }        
        public string Code { get; protected set; }
        public DateTime CreatedDateUtc { get; protected set; }

        public Card() {}

        public Card(Event ev)
        {
            EventId = ev.EventId;
            CreatedDateUtc = DateTime.UtcNow;
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
