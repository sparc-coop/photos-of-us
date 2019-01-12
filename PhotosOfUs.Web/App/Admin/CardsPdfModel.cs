using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Pages.Admin
{
    public class CardsPdfModel
    {
        public string Code { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public CardsPdfModel(Card card, string eventName, User photographer)
        {
            Code = card.Code;
            Url = eventName.ToLower() + ".photosof.us";
            Name = photographer.DisplayName;
            Email = photographer.Email;
        }
    }
}