using Kuvio.Kernel.Architecture;
using Kuvio.Kernel.Auth;
using PhotosOfUs.Model.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;

namespace PhotosOfUs.Pages.Admin
{
    public class CardsModel : PageModel
    {
        private readonly IRepository<Event> _events;

        public CardsModel(IRepository<Event> events)
        {
            _events = events;
        }

        public int EventId { get; private set; }

        public IActionResult OnGet(int eventId)
        {
            var ev = _events.Find(x => x.EventId == eventId && x.UserId == User.ID());
            if (ev == null) return NotFound();

            EventId = eventId;
            return Page();
        }

        public IActionResult OnPost(int eventId, List<int> cardIds)
        {
            var ev = _events.Include(x => x.Cards).Find(x => x.EventId == eventId && x.UserId == User.ID());
            if (ev == null || cardIds == null || !cardIds.Any()) return Unauthorized();
            
            var cards = ev.Cards.Where(x => cardIds.Contains(x.Id))
                .ToList()
                .Select(x => new CardsPdfModel(x, ev.Url, ev.User));

            var json = JsonConvert.SerializeObject(cards);
            return new ViewAsPdf("CardsPdf", new { json })
            {
                FileName = "Cards.pdf",
                PageSize = Size.Letter,
                PageOrientation = Orientation.Landscape,
                PageMargins = { Left = 0, Right = 0 },
                Cookies = Request.Cookies.ToDictionary(x => x.Key, x => x.Value)
            };

        }
    }
}